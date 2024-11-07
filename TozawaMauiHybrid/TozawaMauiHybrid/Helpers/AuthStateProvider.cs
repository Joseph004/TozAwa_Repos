
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Features;
using Microsoft.JSInterop;
using System.Globalization;
using TozawaMauiHybrid.Configurations;
using TozawaMauiHybrid.Services;

namespace TozawaMauiHybrid.Helpers;

public class AuthStateProvider(PreferencesStoreClone storage, AppSettings appSettings) : AuthenticationStateProvider
{
    private readonly PreferencesStoreClone _storage = storage;
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly AppSettings _appSettings = appSettings;
    public void SetFirstLoad(bool firstLoad) => _firstLoad = firstLoad;
    private bool _firstLoad = false;
    public event EventHandler<EventArgs> UserAuthenticationChanged;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (!_firstLoad) return _anonymous;

            string token = null;
            try
            {
                token = _storage.Get<string>("authToken");
            }
            catch (Exception)
            {
                token = null;
            }
            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"))));
        }
        catch (JSDisconnectedException)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }
    public async Task<CurrentUserDto> GetUserFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            token = _storage.Get<string>("authToken");
        }

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
    public void RemoveCurrentUser()
    {
        if (_storage.Exists("currentUser"))
        {
            _storage.Delete("currentUser");
        }
    }
    public void SetCurrentUser(CurrentUserDto user)
    {
        if (_storage.Exists("currentUser"))
        {
            _storage.Delete("currentUser");
        }
        _storage.Set("currentUser", user);
    }
    public async Task NotifyUserAuthentication()
    {
        var token = _storage.Get<string>("authToken");

        var refreshAt = DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(_appSettings.JWTSettings.ExpiryInMinutes)).ToString(CultureInfo.InvariantCulture);
        var user = await GetUserFromToken(token);
        SetCurrentUser(user);
        var claims = new List<Claim>
        {
            new(nameof(CurrentUserDto), await GetUserTokenWhenUserIsAuthenticate()),
            new(ClaimTypes.Email, string.IsNullOrEmpty(user.Email) ? "" : user.Email),
            new(ClaimTypes.Name, user.UserName),
            new("refresh_at", refreshAt),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("exp", _appSettings.JWTSettings.ExpiryInMinutes)
        };
        if (user.Admin)
        {
            claims.Add(new Claim("admin-member", "MemberIsAdmin"));
        }
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
        UserAuthenticationChanged(this, new EventArgs());
    }
    public void NotifyUserLogout()
    {
        RemoveCurrentUser();
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
        UserAuthenticationChanged(this, new EventArgs());
    }
}

