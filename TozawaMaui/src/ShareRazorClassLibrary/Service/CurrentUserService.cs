using Blazored.SessionStorage;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace ShareRazorClassLibrary.Services;
public class CurrentUserService(
    IAuthHttpClient client,
    ISessionStorageService sessionStorageService,
    ISnackBarService snackBarService,
    AuthenticationStateProvider authenticationStateProvider,
    ILogger<CurrentUserService> logger) : ICurrentUserService
{
    private readonly IAuthHttpClient _client = client;
    private readonly ISnackBarService _snackBarService = snackBarService;
    private readonly ISessionStorageService _sessionStorageService = sessionStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;


    private readonly ILogger<CurrentUserService> _logger = logger;

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
    public async Task<CurrentUserDto> GetCurrentUser()
    {
        try
        {
            if (await _sessionStorageService.ContainKeyAsync("currentUser"))
            {
                var response = await _sessionStorageService.GetItemAsync<CurrentUserDto>("currentUser");
                if (response != null && response.Id != Guid.Empty)
                {
                    return response;
                }
            }

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity.IsAuthenticated)
            {
                //_logger.LogError("User is not authenticated");
                return new CurrentUserDto();
            }

            var userString = user.Claims.Where(x => x.Type == nameof(CurrentUserDto)).Select(c => c.Value).SingleOrDefault();

            return JsonConvert.DeserializeObject<CurrentUserDto>(userString);
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        return new CurrentUserDto();
    }

    public async Task<bool> HasRole(string role)
    {
        if (!await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await GetCurrentUser();
        }

        var currentUser = await _sessionStorageService.GetItemAsync<CurrentUserDto>("currentUser");
        return currentUser.Roles.Any(r => r == GetRole(role));
    }
    private static RoleDto GetRole(string role)
    {
        Enum.TryParse(role, out RoleDto myRole);

        return myRole;
    }
}
