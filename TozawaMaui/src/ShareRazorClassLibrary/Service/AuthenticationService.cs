using Microsoft.AspNetCore.Components.Authorization;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.FormModels;
using ShareRazorClassLibrary.Models.ResponseRequests;

namespace ShareRazorClassLibrary.Services;

public partial class AuthenticationService(IAuthHttpClient authHttpClient)
{
    private readonly IAuthHttpClient _authHttpClient = authHttpClient;

    public async Task<AddResponse<LoginResponseDto>> GetLoggedInMember(Guid id) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/loggedin/{id}", null);
    public async Task<AddResponse<LoginResponseDto>> PostLoginMember(LoginRequest command) => await _authHttpClient.SendPost<LoginResponseDto>("authenticate/member/login", command);
    public async Task<AddResponse<LoginResponseDto>> PostLogout(Guid id) => await _authHttpClient.SendPost<LoginResponseDto>($"token/logout/{id.ToString()}", null);
    public async Task<AddResponse<LoginResponseDto>> CheckLockout(string userName) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
