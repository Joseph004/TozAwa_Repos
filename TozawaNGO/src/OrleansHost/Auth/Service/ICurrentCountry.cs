
namespace OrleansHost.Auth.Services;

public interface ICurrentCountry
{
    Task<IPCountryResponse> GetUserCountryByIp();
}