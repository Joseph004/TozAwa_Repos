using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TozAwaHome.HttpClients;
using TozAwaHome.Models.Dtos;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TozAwaHome.Services;
public class CurrentUserService : ICurrentUserService
{
    private readonly IAuthHttpClient _client;


    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        IAuthHttpClient client,
        ILogger<CurrentUserService> logger)
    {
        _client = client;
        _logger = logger;
    }
    public async Task RemoveCurrentUser()
    {
        if (!string.IsNullOrEmpty(await SecureStorage.GetAsync(nameof(CurrentUserDto))))
        {
            SecureStorage.Remove(nameof(CurrentUserDto));
        }
    }
    public async Task SetCurrentUser(CurrentUserDto user)
    {
        if (!string.IsNullOrEmpty(await SecureStorage.GetAsync(nameof(CurrentUserDto))))
        {
            SecureStorage.Remove(nameof(CurrentUserDto));
        }
        await SecureStorage.SetAsync(nameof(CurrentUserDto), JsonConvert.SerializeObject(user));
    }
    public async Task<CurrentUserDto> GetCurrentUser()
    {
        try
        {
            if (!string.IsNullOrEmpty(await SecureStorage.GetAsync(nameof(CurrentUserDto))))
            {
                var response = JsonConvert.DeserializeObject<CurrentUserDto>(await SecureStorage.GetAsync(nameof(CurrentUserDto)));
                if (response != null && response.Id != Guid.Empty)
                {
                    return response;
                }
            }

            /*var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity.IsAuthenticated)
            {
                _logger.LogError("User is not authenticated");
                return new CurrentUserDto();
            }

            var oid = user.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Select(c => c.Value).SingleOrDefault();

            var currentUserResponse = await _client.SendGet<CurrentUserDto>($"authenticate/current/{oid}");
            if (!currentUserResponse.Success)
            {
                await App.Current.MainPage.DisplayAlert("Oops", "Something went wrong, please contact support", "OK");
                return new CurrentUserDto();
            }
            var currentUser = currentUserResponse.Entity ?? new CurrentUserDto();
            await SecureStorage.SetAsync(nameof(CurrentUserDto), JsonConvert.SerializeObject(currentUser));

            return currentUser;*/
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        return new CurrentUserDto();
    }
    public async Task<bool> HasAtLeastOneFeature(List<int> features)
    {
        if (!!string.IsNullOrEmpty(await SecureStorage.GetAsync(nameof(CurrentUserDto))))
        {
            await GetCurrentUser();
        }

        var currentUser = JsonConvert.DeserializeObject<CurrentUserDto>(await SecureStorage.GetAsync(nameof(CurrentUserDto)));
        return currentUser.Features.Any(f => features.Contains(f));
    }

    public async Task<bool> HasFunctionType(string functionType)
    {
        if (!!string.IsNullOrEmpty(await SecureStorage.GetAsync(nameof(CurrentUserDto))))
        {
            await GetCurrentUser();
        }

        var currentUser = JsonConvert.DeserializeObject<CurrentUserDto>(await SecureStorage.GetAsync(nameof(CurrentUserDto)));
        return currentUser.Functions.Any(f => f.FunctionType.Equals(functionType, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> HasFeature(int feature)
    {
        if (!!string.IsNullOrEmpty(await SecureStorage.GetAsync(nameof(CurrentUserDto))))
        {
            await GetCurrentUser();
        }

        var currentUser = JsonConvert.DeserializeObject<CurrentUserDto>(await SecureStorage.GetAsync(nameof(CurrentUserDto)));
        return currentUser.Features.Contains(feature);
    }
}
