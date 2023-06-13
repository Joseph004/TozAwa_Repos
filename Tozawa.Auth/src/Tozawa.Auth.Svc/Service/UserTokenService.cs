
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Tozawa.Auth.Svc.Configurations;
using Tozawa.Auth.Svc.extension;
using Tozawa.Auth.Svc.Models.Dtos;

namespace Tozawa.Auth.Svc.Services;

public interface IUserTokenService
{
    JwtSecurityToken GenerateTokenOptions(CurrentUserDto user);
    CurrentUserDto GenerateUseFromToken(string token);
    string GenerateRefreshToken();
    CurrentUserDto GetUserFromExpiredToken(string token);
}

public class UserTokenService : IUserTokenService
{
    private AppSettings _appSettings;
    public UserTokenService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

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
        var claims = new List<Claim>
    {
        new Claim(nameof(CurrentUserDto), JsonConvert.SerializeObject(user))
    };

        return claims;
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
        var claimValue = principal.Identity.Name;

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