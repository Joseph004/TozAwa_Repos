using TozawaMauiHybrid.HttpClients;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Models.ResponseRequests;

namespace TozawaMauiHybrid.Services;

public partial class AuthenticationService(IAuthHttpClient authHttpClient)
{
    private readonly IAuthHttpClient _authHttpClient = authHttpClient;

    public async Task<GetResponse<byte[]>> GetCert()
    {
        var uri = $"tzkey";
        return await _authHttpClient.SendGet<byte[]>(uri);
    }
    public async Task<AddResponse<LoginResponseDto>> GetLoggedIn(Guid id) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/getUser/{id}", null);
    public async Task<AddResponse<LoginResponseDto>> PostLogin(LoginRequest command) => await _authHttpClient.SendPost<LoginResponseDto>("authenticate/signin", command);
    public async Task<AddResponse<LoginResponseDto>> PostLogout(Guid id) => await _authHttpClient.SendPost<LoginResponseDto>($"token/logout/{id.ToString()}", null);
    public async Task<AddResponse<LoginResponseDto>> CheckLockout(string userName) => await _authHttpClient.SendPost<LoginResponseDto>($"authenticate/root/{userName}", new object());
}
