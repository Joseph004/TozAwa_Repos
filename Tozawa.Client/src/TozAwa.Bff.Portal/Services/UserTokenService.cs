
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace Tozawa.Bff.Portal.Services;

public interface IUserTokenService
{
    JwtSecurityToken GenerateTokenOptions(string token);
    JwtSecurityToken GenerateTokenOptionsForAthService(string token);
}

public class UserTokenService : IUserTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationSection _jwtSettings;
    public UserTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtSettings = _configuration.GetSection("JwtSettings");
    }

    private SigningCredentials GetSigningCredentials(bool isForAuthService = false)
    {
        var key = Encoding.UTF8.GetBytes(isForAuthService ? _jwtSettings["securityKeyToAuthService"] : _jwtSettings["securityKey"]);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public JwtSecurityToken GenerateTokenOptions(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var signingCredentials = GetSigningCredentials();

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings["validIssuer"],
            audience: _jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
            signingCredentials: signingCredentials);

        return tokenOptions;
    }
    public JwtSecurityToken GenerateTokenOptionsForAthService(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var signingCredentials = GetSigningCredentials(true);

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings["validIssuer"],
            audience: _jwtSettings["validAudienceToAuthService"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
            signingCredentials: signingCredentials);

        return tokenOptions;
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
}