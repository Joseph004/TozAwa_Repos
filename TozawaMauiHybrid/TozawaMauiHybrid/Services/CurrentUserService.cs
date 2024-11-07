using TozawaMauiHybrid.HttpClients;
using TozawaMauiHybrid.Models.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using TozawaMauiHybrid.Helpers;

namespace TozawaMauiHybrid.Services;
public class CurrentUserService(
    IAuthHttpClient client,
    PreferencesStoreClone storage,
    AuthenticationStateProvider authenticationStateProvider,
    ILogger<CurrentUserService> logger) : ICurrentUserService
{
    private readonly IAuthHttpClient _client = client;
    private readonly PreferencesStoreClone _storage = storage;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;


    private readonly ILogger<CurrentUserService> _logger = logger;

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
    public async Task<CurrentUserDto> GetCurrentUser()
    {
        try
        {
            if (_storage.Exists("currentUser"))
            {
                var response = _storage.Get<CurrentUserDto>("currentUser");
                if (response != null && response.Id != Guid.Empty)
                {
                    return response;
                }
            }

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity == null || !user.Identity.IsAuthenticated)
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
        if (!_storage.Exists("currentUser"))
        {
            await GetCurrentUser();
        }

        var currentUser = _storage.Get<CurrentUserDto>("currentUser");
        return currentUser.Roles.Any(r => r == GetRole(role));
    }
    private static RoleDto GetRole(string role)
    {
        Enum.TryParse(role, out RoleDto myRole);

        return myRole;
    }
}
