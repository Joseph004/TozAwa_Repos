
namespace TozawaNGO.Auth.Services;

public interface ICurrentCountry
{
    Task<IPCountryResponse> GetUserCountryByIp();
}