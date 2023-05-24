using System.Net;
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Tozawa.Language.Client.Helpers;
using Tozawa.Language.Client.HttpClients;
using Tozawa.Language.Client.Models.Dtos;
using Tozawa.Language.Client.Models.ResponseRequests;

namespace Tozawa.Language.Client.Services;
public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationService _client;
    private readonly ISnackBarService _snackBarService;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;


    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        AuthenticationService client,
        ISessionStorageService sessionStorageService,
        ISnackBarService snackBarService,
        AuthenticationStateProvider authenticationStateProvider,
        ILogger<CurrentUserService> logger)
    {
        _client = client;
        _sessionStorageService = sessionStorageService;
        _authenticationStateProvider = authenticationStateProvider;
        _snackBarService = snackBarService;
        _logger = logger;
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
    public async Task<ClaimsPrincipal> GetAuthenticationStateAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User;
    }
    public async Task<CurrentUserDto> GetCurrentUser()
    {
        try
        {
            if (await _sessionStorageService.ContainKeyAsync("currentUser"))
            {
                return await _sessionStorageService.GetItemAsync<CurrentUserDto>("currentUser");
            }

            var user = await GetAuthenticationStateAsync();

            if (!user.Identity.IsAuthenticated)
            {
                _logger.LogError("User is not authenticated");
                _snackBarService.Add(new GetResponse<CurrentUserDto>(false, StatusTexts.GetHttpStatusText(HttpStatusCode.Unauthorized), HttpStatusCode.Unauthorized, null));
                return new CurrentUserDto();
            }

            var oid = user.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Select(c => c.Value).SingleOrDefault();

            var currentUserResponse = await _client.GetCurrentUser(Guid.Parse(oid));
            if (!currentUserResponse.Success)
            {
                _snackBarService.Add(currentUserResponse);
                return new CurrentUserDto();
            }
            var currentUser = currentUserResponse.Entity ?? new CurrentUserDto();
            await _sessionStorageService.SetItemAsync("currentUser", currentUser);

            return currentUser;
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        return new CurrentUserDto();
    }
    public async Task<bool> HasAtLeastOneFeature(List<int> features)
    {
        if (!await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await GetCurrentUser();
        }

        var currentUser = await _sessionStorageService.GetItemAsync<CurrentUserDto>("currentUser");
        return currentUser.Features.Any(f => features.Contains(f));
    }

    public async Task<bool> HasFunctionType(string functionType)
    {
        if (!await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await GetCurrentUser();
        }

        var currentUser = await _sessionStorageService.GetItemAsync<CurrentUserDto>("currentUser");
        return currentUser.Functions.Any(f => f.FunctionType.Equals(functionType, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> HasFeature(int feature)
    {
        if (!await _sessionStorageService.ContainKeyAsync("currentUser"))
        {
            await GetCurrentUser();
        }

        var currentUser = await _sessionStorageService.GetItemAsync<CurrentUserDto>("currentUser");
        return currentUser.Features.Contains(feature);
    }
}
