using Microsoft.AspNetCore.Components.Authorization;
using TozawaNGO.HttpClients;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.FormModels;
using TozawaNGO.Models.ResponseRequests;

namespace TozawaNGO.Services;

public partial class AuthenticationService(AuthenticationStateProvider authenticationStateProvider, IAuthHttpClient authHttpClient)
{
    private readonly IAuthHttpClient _authHttpClient = authHttpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

    public async Task<AddResponse<LoginResponseDto>> PostLogin(LoginRequest command) => await _authHttpClient.SendPost<LoginResponseDto>("authenticate/signin", command);
    public async Task<AddResponse<LoginResponseDto>> CheckLockout(string userName) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
