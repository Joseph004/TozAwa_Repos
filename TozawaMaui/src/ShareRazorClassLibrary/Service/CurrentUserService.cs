using Blazored.SessionStorage;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using ShareRazorClassLibrary.Helpers;
using ShareRazorClassLibrary.Models.Enums;

namespace ShareRazorClassLibrary.Services;
public class CurrentUserService(
    IAuthHttpClient client,
    ISnackBarService snackBarService,
    AuthStateProvider authStateProvider,
    ILogger<CurrentUserService> logger) : ICurrentUserService
{
    private readonly IAuthHttpClient _client = client;
    private readonly ISnackBarService _snackBarService = snackBarService;
    private readonly AuthStateProvider _authStateProvider = authStateProvider;
    private readonly ILogger<CurrentUserService> _logger = logger;

    public async Task<CurrentUserDto> GetCurrentUser()
    {
        try
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();

            if (authState.User.Identity == null || !authState.User.Identity.IsAuthenticated)
            {
                //_logger.LogError("User is not authenticated");
                return new CurrentUserDto();
            }
            var userString = authState.User.Claims.Where(x => x.Type == nameof(CurrentUserDto)).Select(c => c.Value).SingleOrDefault();

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
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity == null || !authState.User.Identity.IsAuthenticated)
        {
            return false;
        }
        var currentUser = await GetCurrentUser();
        return currentUser.Features.Any(f => features.Contains(f));
    }

    public async Task<bool> HasFunctionType(string functionType)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity == null || !authState.User.Identity.IsAuthenticated)
        {
            return false;
        }
        var currentUser = await GetCurrentUser();
        return currentUser.Functions.Any(f => Enum.GetName(typeof(FunctionType), f.FunctionType).Equals(functionType, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> HasFeature(int feature)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity == null || !authState.User.Identity.IsAuthenticated)
        {
            return false;
        }
        var currentUser = await GetCurrentUser();
        return currentUser.Features.Contains(feature);
    }
}
