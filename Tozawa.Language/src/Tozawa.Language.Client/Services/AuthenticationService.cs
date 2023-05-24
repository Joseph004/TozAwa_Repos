using Microsoft.AspNetCore.Components.Authorization;
using Tozawa.Language.Client.HttpClients;
using Tozawa.Language.Client.Models.Dtos;
using Tozawa.Language.Client.Models.DTOs;
using Tozawa.Language.Client.Models.FormModels;
using Tozawa.Language.Client.Models.ResponseRequests;

namespace Tozawa.Language.Client.Services;

public partial class AuthenticationService
{
    private readonly IAuthHttpClient _authHttpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationService(AuthenticationStateProvider authenticationStateProvider, IAuthHttpClient authHttpClient)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _authHttpClient = authHttpClient;
    }

    public async Task<LoginResponseDto> PostLogin(LoginRequest command) => await _authHttpClient.SendNoResponsePost<LoginResponseDto>("authenticate/root", command);
    public async Task<GetResponse<CurrentUserDto>> GetCurrentUser(Guid id) => await _authHttpClient.SendGet<CurrentUserDto>($"authenticate/current/{id}");
    public async Task<LoginResponseDto> CheckLockout(string userName) => await _authHttpClient.SendNoResponsePost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
