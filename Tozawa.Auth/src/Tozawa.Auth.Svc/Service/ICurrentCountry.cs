
namespace Tozawa.Auth.Svc.Services;

public interface ICurrentCountry
{
    Task<IPCountryResponse> GetUserCountryByIp();
}