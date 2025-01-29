
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Features;
using Microsoft.JSInterop;
using System.Globalization;
using TozawaMauiHybrid.Configurations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TozawaMauiHybrid.Helpers;

public class AuthStateProvider(PreferencesStoreClone storage, AppSettings appSettings) : AuthenticationStateProvider
{
    private readonly PreferencesStoreClone _storage = storage;
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly AppSettings _appSettings = appSettings;
    public event EventHandler<EventArgs> UserAuthenticationChanged;
    public UserLoginStateDto UserLoginStateDto { get; } = new();
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(UserLoginStateDto.JWTToken) || !UserLoginStateDto.IsAuthenticated)
                return _anonymous;

            if (UserLoginStateDto.IsAuthenticated && !string.IsNullOrWhiteSpace(UserLoginStateDto.JWTToken))
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(UserLoginStateDto.JWTToken), "tzuserauthentication"))));
        }
        catch (JSDisconnectedException)
        {
            return _anonymous;
        }
        return _anonymous;
    }
    public async Task<CurrentUserDto> GetUserFromToken()
    {
        var authUser = await GetAuthenticationStateAsync();
        if (!authUser.User.Claims.Any()) return new CurrentUserDto();

        var userString = authUser.User.Claims.Where(x => x.Type == nameof(CurrentUserDto)).Select(c => c.Value).SingleOrDefault();

        return JsonConvert.DeserializeObject<CurrentUserDto>(userString);
    }
    private async Task<string> GetUserTokenWhenUserIsAuthenticate()
    {
        var authUser = await GetAuthenticationStateAsync();
        if (!authUser.User.Claims.Any()) return string.Empty;

        return authUser.User.Claims.Where(x => x.Type == nameof(CurrentUserDto)).Select(c => c.Value).SingleOrDefault();
    }
    public void NotifyUserAuthentication(List<Claim> claims)
    {
        var authenticatedUser = new ClaimsPrincipal();
        if (claims != null && claims.Count > 0)
        {
            var token = claims.Where(x => x.Type == "auth_token").Select(c => c.Value).SingleOrDefault();
            var refreshToken = claims.Where(x => x.Type == "auth_refreshtoken").Select(c => c.Value).SingleOrDefault();

            UserLoginStateDto.Set(true, token, refreshToken, UserLoginStateDto.WorkOrganizationId);

            authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "tzuserauthentication"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
            var userString = claims.Where(x => x.Type == nameof(CurrentUserDto)).Select(c => c.Value).SingleOrDefault();
            var user = JsonConvert.DeserializeObject<CurrentUserDto>(userString);
            _storage.Set("auth_loggedIn", Cryptography.Encrypt(user.Id.ToString(), "PG=?1PowK<ai57:t%`Ro}L9~1q2&-i/H", "HK2nvSMadZRDeTbB"));
            UserAuthenticationChanged(this, new EventArgs());
        }
        else
        {
            UserLoginStateDto.Set(false, null, null, Guid.Empty);
        }
    }
    public async Task NotifyUserAuthentication(string token, string refreshToken)
    {
        var authenticatedUser = new ClaimsPrincipal();
        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
        {
            UserLoginStateDto.Set(true, token, refreshToken, UserLoginStateDto.WorkOrganizationId);
            var refreshAt = DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(_appSettings.JWTSettings.ExpiryInMinutes)).ToString(CultureInfo.InvariantCulture);
            var user = await GetUserFromToken();
            var claims = new List<Claim>
        {
            new(nameof(CurrentUserDto), await GetUserTokenWhenUserIsAuthenticate()),
            new(ClaimTypes.Email, string.IsNullOrEmpty(user.Email) ? "" : user.Email),
            new(ClaimTypes.Name, user.UserName),
            new("refresh_at", refreshAt),
            new("auth_token", token),
            new("auth_refreshtoken", refreshToken),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("exp", _appSettings.JWTSettings.ExpiryInMinutes)
        };
            if (user.Roles.Count >= 1)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, Enum.GetName(typeof(Role), role.Role)));
                }
            }
            if (user.Admin)
            {
                claims.Add(new Claim("admin-member", "MemberIsAdmin"));
            }
            authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "tzuserauthentication"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
            _storage.Set("auth_loggedIn", Cryptography.Encrypt(user.Id.ToString(), "PG=?1PowK<ai57:t%`Ro}L9~1q2&-i/H", "HK2nvSMadZRDeTbB"));
            UserAuthenticationChanged(this, new EventArgs());
        }
        else
        {
            UserLoginStateDto.Set(false, null, null, Guid.Empty);
        }
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
    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
        UserLoginStateDto.Set(false, null, null, Guid.Empty);
        _storage.Delete("auth_loggedIn");
        UserAuthenticationChanged(this, new EventArgs());
    }
}