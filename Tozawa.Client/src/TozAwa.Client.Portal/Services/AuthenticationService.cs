using Microsoft.AspNetCore.Components.Authorization;
using Tozawa.Client.Portal.HttpClients;
using Tozawa.Client.Portal.Models.Dtos;
using Tozawa.Client.Portal.Models.FormModels;
using Tozawa.Client.Portal.Models.ResponseRequests;

namespace TozAwa.Client.Portal.Services;

public partial class AuthenticationService
{
    private readonly IAuthHttpClient _authHttpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationService(AuthenticationStateProvider authenticationStateProvider, IAuthHttpClient authHttpClient)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _authHttpClient = authHttpClient;
    }

    public async Task<AddResponse<LoginResponseDto>> PostLogin(LoginRequest command) => await _authHttpClient.SendPost<LoginResponseDto>("authenticate/signin", command);
    public async Task<AddResponse<LoginResponseDto>> CheckLockout(string userName) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
