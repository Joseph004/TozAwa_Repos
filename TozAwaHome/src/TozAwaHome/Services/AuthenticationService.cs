using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using TozAwaHome.HttpClients;
using TozAwaHome.Models.Dtos;
using TozAwaHome.Models.FormModels;
using TozAwaHome.Models.ResponseRequests;

namespace TozAwaHome.Services;

public partial class AuthenticationService
{
    private readonly IAuthHttpClient _authHttpClient;

    public AuthenticationService(IAuthHttpClient authHttpClient)
    {
        _authHttpClient = authHttpClient;
    }

    public async Task<AddResponse<LoginResponseDto>> PostLogin(LoginRequest command) => await _authHttpClient.SendPost<LoginResponseDto>("authenticate/signin", command);
    public async Task<AddResponse<LoginResponseDto>> CheckLockout(string userName) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
