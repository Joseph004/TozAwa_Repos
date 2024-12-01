using Microsoft.AspNetCore.Components.Authorization;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Models.ResponseRequests;

namespace ShareRazorClassLibrary.Services;

public partial class AuthenticationService(AuthenticationStateProvider authenticationStateProvider, IAuthHttpClient authHttpClient)
{
    private readonly IAuthHttpClient _authHttpClient = authHttpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

    public async Task<AddResponse<LoginResponseDto>> GetLoggedIn(Guid id) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/getUser/{id}", null);
    public async Task<AddResponse<LoginResponseDto>> PostLogin(LoginRequest command) => await _authHttpClient.SendPost<LoginResponseDto>("authenticate/signin", command);
    public async Task<AddResponse<LoginResponseDto>> PostLogout(Guid id) => await _authHttpClient.SendPost<LoginResponseDto>($"token/logout/{id.ToString()}", null);
    public async Task<AddResponse<LoginResponseDto>> CheckLockout(string userName) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
