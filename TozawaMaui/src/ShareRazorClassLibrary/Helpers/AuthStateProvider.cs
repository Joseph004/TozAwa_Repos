
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Features;
using Microsoft.JSInterop;
using System.Globalization;
using Blazored.LocalStorage;
using ShareRazorClassLibrary.Configurations;
using ShareRazorClassLibrary.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ShareRazorClassLibrary.Helpers;

public class AuthStateProvider(ISessionStorageService sessionStorageService, ILocalStorageService localStorage, AppSettings appSettings, FirsloadState firsloadState) : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionStorageService = sessionStorageService;
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly FirsloadState _firsloadState = firsloadState;
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly AppSettings _appSettings = appSettings;

    public event EventHandler<EventArgs> UserAuthenticationChanged;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_firsloadState.IsFirstLoaded)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        try
        {
            string token = null;
            try
            {
                token = await _localStorage.GetItemAsync<string>("authToken");
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
            token = await _localStorage.GetItemAsync<string>("authToken");
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
    public async Task RemoveCurrentUser()
    {
        if (!_firsloadState.IsFirstLoaded)
        {
            return;
        }
        if (await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await _sessionStorageService.RemoveItemAsync("currentUser");
        }
    }
    public async Task SetCurrentUser(CurrentUserDto user)
    {
        if (!_firsloadState.IsFirstLoaded)
        {
            return;
        }
        if (await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await _sessionStorageService.RemoveItemAsync("currentUser");
        }
        await _sessionStorageService.SetItemAsync("currentUser", user);
    }
    public async void NotifyUserAuthentication()
    {
        if (!_firsloadState.IsFirstLoaded)
        {
            return;
        }
        var token = await _localStorage.GetItemAsync<string>("authToken");

        var refreshAt = DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(_appSettings.JWTSettings.ExpiryInMinutes)).ToString(CultureInfo.InvariantCulture);
        var user = await GetUserFromToken(token);
        await SetCurrentUser(user);
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

    public async void NotifyUserLogout()
    {
        if (!_firsloadState.IsFirstLoaded)
        {
            return;
        }
        await RemoveCurrentUser();
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
        UserAuthenticationChanged(this, new EventArgs());
    }
}

