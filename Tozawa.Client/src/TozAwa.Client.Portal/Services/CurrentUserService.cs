using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.Extensions.Logging;
using Tozawa.Client.Portal.HttpClients;
using Tozawa.Client.Portal.Models.Dtos;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Tozawa.Client.Portal.Services;
public class CurrentUserService : ICurrentUserService
{
    private readonly IAuthHttpClient _client;
    private readonly ISnackBarService _snackBarService;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;


    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        IAuthHttpClient client,
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
