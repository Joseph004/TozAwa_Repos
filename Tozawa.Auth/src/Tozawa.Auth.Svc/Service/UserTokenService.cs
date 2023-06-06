
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Tozawa.Auth.Svc.extension;
using Tozawa.Auth.Svc.Models.Dtos;

namespace Tozawa.Auth.Svc.Services;

public interface IUserTokenService
{
    JwtSecurityToken GenerateTokenOptions(CurrentUserDto user);
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

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    /* private List<Claim> GetFunctionClaims(FunctionDto function)
    {
        var claims = new List<Claim>
        {
          new Claim("RoleId", function.RoleId.ToString()),
          new Claim("FunctionType", function.FunctionType.ToString()),
          new Claim("FunctionTypeValues", JsonConvert.SerializeObject(GetFunctionTypeValuesClaims(function.FunctionTypeValues)))
        };

        return claims;
    }
    private List<Claim> GetFunctionTypeValuesClaims(FunctionTypeValues functionTypeValues)
    {
        var claims = new List<Claim>
        {
            new Claim("PairId", functionTypeValues.PairId),
            new Claim("OnlyVisibleForRoot", functionTypeValues.OnlyVisibleForRoot.ToString(), ClaimValueTypes.Boolean),
            new Claim("Obsolete", functionTypeValues.Obsolete.ToString(), ClaimValueTypes.Boolean),
            new Claim("AccessType", functionTypeValues.AccessType.ToString())
        };

        return claims;
    }
    private List<Claim> GetUserFunction(CurrentUserFunctionDto currentUserFunction)
    {
        return new List<Claim>
        {
          new Claim("FunctionType", currentUserFunction.FunctionType.ToString()),
          new Claim("TextId", currentUserFunction.TextId.ToString())
        };
    }
    private List<Claim> GetRoleClaims(CurrentUserRoleDto role)
    {
        return new List<Claim>
        {
          new Claim("TextId", role.TextId.ToString()),
          new Claim("OrganizationId", role.OrganizationId.ToString()),
          new Claim("FunctionTypeValues", JsonConvert.SerializeObject(role.Functions.Select(x => GetFunctionClaims(x))))
        };
    } */
    private List<Claim> GetClaims(CurrentUserDto user)
    {
        var claims = new List<Claim>
    {
        new Claim(nameof(CurrentUserDto), JsonConvert.SerializeObject(user))
       /*  new Claim("Email", user.Email),
        new Claim("FirstName", user.FirstName),
        new Claim("LastName", user.LastName),
        new Claim("RootUser", user.RootUser.ToString(), ClaimValueTypes.Boolean),
        new Claim("Roles", JsonConvert.SerializeObject(user.Roles.Select(x => GetRoleClaims(x)))),
        new Claim("Functions", JsonConvert.SerializeObject(user.Functions.Select(x => GetUserFunction(x)))),
        new Claim("OrganizationId", user.OrganizationId.ToString()),
        new Claim("FallbackLanguageId", user.FallbackLanguageId.ToString()),
        new Claim("LanguageId", user.LanguageId.ToString()),
        new Claim("Organization", user.Organization),
        new Claim("Id", user.Id.ToString()),
        new Claim("Oid", user.Oid.ToString()), */
    };

        return claims;
    }
    public JwtSecurityToken GenerateTokenOptions(CurrentUserDto user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = GetClaims(user);

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings["validIssuer"],
            audience: _jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
            signingCredentials: signingCredentials);

        return tokenOptions;
    }
}