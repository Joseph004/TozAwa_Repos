using Microsoft.AspNetCore.Components.Authorization;
using TozawaMauiHybrid.HttpClients;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Models.ResponseRequests;

namespace TozawaMauiHybrid.Services;

public partial class AuthenticationService(AuthenticationStateProvider authenticationStateProvider, IAuthHttpClient authHttpClient)
{
    private readonly IAuthHttpClient _authHttpClient = authHttpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

    public async Task<AddResponse<LoginResponseDto>> PostLogin(LoginRequest command) => await _authHttpClient.SendPost<LoginResponseDto>("authenticate/signin", command);
    public async Task<AddResponse<LoginResponseDto>> CheckLockout(string userName) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
