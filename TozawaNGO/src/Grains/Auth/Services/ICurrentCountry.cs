
namespace Grains.Auth.Services;

public interface ICurrentCountry
{
    Task<IPCountryResponse> GetUserCountryByIp();
}