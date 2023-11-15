
using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Features;
using Microsoft.JSInterop;
using TozawaNGO.Auth.Models;
using System.Globalization;

namespace TozawaNGO.Helpers;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly AuthenticationState _anonymous;
    private readonly TokenProvider _tokenProvider;
    public AuthStateProvider(HttpClient httpClient, ISessionStorageService sessionStorageService, TokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _sessionStorageService = sessionStorageService;
        _tokenProvider = tokenProvider;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_tokenProvider.Token))
                return _anonymous;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("tzuserauthentication", _tokenProvider.Token);
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(_tokenProvider.Token), "jwtAuthType"))));
        }
        catch (JSDisconnectedException)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }
    public async Task<CurrentUserDto> GetUserFromToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _tokenProvider.Token = token;
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
        if (await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await _sessionStorageService.RemoveItemAsync("currentUser");
        }
    }
    public async Task SetCurrentUser(CurrentUserDto user)
    {
        if (await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await _sessionStorageService.RemoveItemAsync("currentUser");
        }
        await _sessionStorageService.SetItemAsync("currentUser", user);
    }
    public async void NotifyUserAuthentication()
    {
        var refreshAt = DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(_tokenProvider.ExpiresIn)).ToString(CultureInfo.InvariantCulture);
        var user = await GetUserFromToken(string.Empty);
        await SetCurrentUser(user);
        var claims = new List<Claim>
        {
            new(nameof(CurrentUserDto), await GetUserTokenWhenUserIsAuthenticate()),
            new(ClaimTypes.Email, string.IsNullOrEmpty(user.Email) ? "" : user.Email),
            new(ClaimTypes.Name, user.UserName),
            new("refresh_at", refreshAt),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        if (user.Admin)
        {
            claims.Add(new Claim("admin-member", "MemberIsAdmin"));
        }
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }
    public async void NotifyUserAuthentication(string token)
    {
        var refreshAt = DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(_tokenProvider.ExpiresIn)).ToString(CultureInfo.InvariantCulture);
        var user = await GetUserFromToken(token);
        await SetCurrentUser(user);
        var claims = new List<Claim>
        {
            new(nameof(CurrentUserDto), await GetUserTokenWhenUserIsAuthenticate()),
            new(ClaimTypes.Email, string.IsNullOrEmpty(user.Email) ? "" : user.Email),
            new(ClaimTypes.Name, user.UserName),
            new("refresh_at", refreshAt),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        if (user.Admin)
        {
            claims.Add(new Claim("admin-member", "MemberIsAdmin"));
        }
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }
    public async void NotifyUserLogout()
    {
        await RemoveCurrentUser();
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}

