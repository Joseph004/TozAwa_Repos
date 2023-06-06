
using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Tozawa.Client.Portal.Models.Dtos;
using TozAwa.Client.Portal.Features;

namespace TozAwa.Client.Portal;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous;
    private readonly ISessionStorageService _sessionStorageService;
    public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, ISessionStorageService sessionStorageService)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        _sessionStorageService = sessionStorageService;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("tzuserauthentication", token);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
    }
    private async Task<CurrentUserDto> GetUserFromToken()
    {
        var authUser = await GetAuthenticationStateAsync();
        if (!authUser.User.Claims.Any()) return new CurrentUserDto();

        var userString = authUser.User.Claims.Where(x => x.Type == nameof(CurrentUserDto)).Select(c => c.Value).SingleOrDefault();

        return JsonConvert.DeserializeObject<CurrentUserDto>(userString);
    }
    private async Task RemoveCurrentUser()
    {
        if (await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await _sessionStorageService.RemoveItemAsync("currentUser");
        }
    }
    private async Task SetCurrentUser(CurrentUserDto user)
    {
        if (await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await _sessionStorageService.RemoveItemAsync("currentUser");
        }
        await _sessionStorageService.SetItemAsync("currentUser", user);
    }
    public async void NotifyUserAuthentication(string token)
    {
        var user = await GetUserFromToken();
        await SetCurrentUser(user);
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(nameof(CurrentUserDto), token),
            new Claim("root-user", user.RootUser ? "" : "UserIsRoot"),
            new Claim(ClaimTypes.Email, string.IsNullOrEmpty(user.Email) ? "" : user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
         }, "jwtAuthType"));
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

