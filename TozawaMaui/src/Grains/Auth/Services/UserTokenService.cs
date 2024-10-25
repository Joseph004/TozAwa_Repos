
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Grains.Configurations;
using Grains.Auth.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Grains.Auth.Services;

public interface IUserTokenService
{
    JwtSecurityToken GenerateTokenOptions(CurrentUserDto user);
    CurrentUserDto GenerateUseFromToken(string token);
    bool ValidateCurrentToken(string token);
    bool ValidateCurrentTokenByIgnoreExpire(string token);
    string GenerateRefreshToken();
    CurrentUserDto GetUserFromExpiredToken(string token);
}

public class UserTokenService(AppSettings appSettings) : IUserTokenService
{
    private AppSettings _appSettings = appSettings;

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_appSettings.JWTSettings.SecurityKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    public CurrentUserDto GenerateUseFromToken(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var claimValue = claims.First(x => x.Type == nameof(CurrentUserDto)).Value;

        return System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(claimValue);
    }
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);

        var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
        return claims;
    }
    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
    private List<Claim> GetClaims(CurrentUserDto user)
    {
        var refreshAt = DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(_appSettings.JWTSettings.ExpiryInMinutes)).ToString(CultureInfo.InvariantCulture);
        var exp = DateTime.UtcNow.AddMinutes(_appSettings.JWTSettings.LogoutUserOn).ToString("dd/MM/yyyy hh:mm:ss");
        var claims = new List<Claim>
    {
        new(nameof(CurrentUserDto), JsonConvert.SerializeObject(user)),
        new(ClaimTypes.Email, string.IsNullOrEmpty(user.Email) ? "" : user.Email),
        new("userName", user.UserName),
        new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        new("refresh_at", refreshAt),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new("exp", _appSettings.JWTSettings.ExpiryInMinutes),
        new("logoutexpat", exp)
    };
        if (user.Admin)
        {
            claims.Add(new Claim("admin-member", "MemberIsAdmin"));
        }
        return claims;
    }
    public bool ValidateCurrentTokenByIgnoreExpire(string token)
    {
        var myIssuer = _appSettings.JWTSettings.ValidIssuer;
        var myAudience = _appSettings.JWTSettings.ValidAudience;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = myIssuer,
                ValidAudience = myAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWTSettings.SecurityKey))
            }, out SecurityToken validatedToken);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
    public bool ValidateCurrentToken(string token)
    {
        var myIssuer = _appSettings.JWTSettings.ValidIssuer;
        var myAudience = _appSettings.JWTSettings.ValidAudience;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = myIssuer,
                ValidAudience = myAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWTSettings.SecurityKey))
            }, out SecurityToken validatedToken);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public JwtSecurityToken GenerateTokenOptions(CurrentUserDto user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = GetClaims(user);

        var tokenOptions = new JwtSecurityToken(
            issuer: _appSettings.JWTSettings.ValidIssuer,
            audience: _appSettings.JWTSettings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_appSettings.JWTSettings.ExpiryInMinutes)),
            signingCredentials: signingCredentials);

        return tokenOptions;
    }
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
    public CurrentUserDto GetUserFromExpiredToken(string token)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        var claimValue = principal.Claims.First(x => x.Type == nameof(CurrentUserDto)).Value;

        return System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(claimValue);
    }
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_appSettings.JWTSettings.SecurityKey)),
            ValidateLifetime = false,
            ValidIssuer = _appSettings.JWTSettings.ValidIssuer,
            ValidAudience = _appSettings.JWTSettings.ValidAudience,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}