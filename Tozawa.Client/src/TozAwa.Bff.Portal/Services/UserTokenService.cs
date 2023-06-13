
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Services;

public interface IUserTokenService
{
    CurrentUserDto GenerateUseFromToken(string token);
    bool ValidateCurrentToken(string token);
    string GenerateToken(string token);
    string GetTokenToAuth(string token);
}

public class UserTokenService : IUserTokenService
{
    private readonly AppSettings _appSettings;
    public UserTokenService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public CurrentUserDto GenerateUseFromToken(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var claimValue = claims.First(x => x.Type == nameof(CurrentUserDto)).Value;

        return System.Text.Json.JsonSerializer.Deserialize<CurrentUserDto>(claimValue);
    }
    private Claim GetUserClaimSerialize(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        return claims.First(x => x.Type == nameof(CurrentUserDto));
    }
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
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
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }
    public string GenerateToken(string token)
    {
        var claim = GetUserClaimSerialize(token);
        var mySecret = _appSettings.JWTSettings.SecurityKey;

        var myIssuer = _appSettings.JWTSettings.ValidIssuer;
        var myAudience = _appSettings.JWTSettings.ValidAudience;

        var signingCredentials = GetSigningCredentials();

        var tokenOptions = new JwtSecurityToken(
            issuer: myIssuer,
            audience: myAudience,
            claims: new List<Claim> { claim },
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_appSettings.JWTSettings.ExpiryInMinutes)),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
    public string GetTokenToAuth(string token)
    {
        var claim = GetUserClaimSerialize(token);
        var signingCredentials = GetSigningCredentials(true);

        var tokenOptions = new JwtSecurityToken(
            issuer: _appSettings.JWTSettings.ValidIssuerForAuth,
            audience: _appSettings.JWTSettings.ValidAudienceForAuth,
            claims: new List<Claim> { claim },
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_appSettings.JWTSettings.ExpiryInMinutes)),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
    private SigningCredentials GetSigningCredentials(bool isForAuth = false)
    {
        var key = Encoding.UTF8.GetBytes(isForAuth ? _appSettings.JWTSettings.SecurityKeyForAuth : _appSettings.JWTSettings.SecurityKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
}