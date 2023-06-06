
using Newtonsoft.Json;
using IpInfo.Api.Client;
using IpInfo.Api.Client.Models;
using System.Globalization;

namespace Tozawa.Bff.Portal.Services;

public interface IUserCountryByIp
{
    Task<string> GetUserCountryByIp();
}

public class UserCountryByIp : IUserCountryByIp
{
    public UserCountryByIp()
    {

    }

    public async Task<string> GetUserCountryByIp()
    {
        var ipInfo = new GetIpInfoResponse();
        try
        {
            var httpClient = new HttpClient();
            var ipResponse = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, "https://ipv4.icanhazip.com/"));

            var jsonString = await ipResponse.Content.ReadAsStringAsync();

            var ip = jsonString.Replace("\n", "").Replace("\r", "");
            if (!string.IsNullOrEmpty(ip))
            {
                try
                {
                    var response = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, "http://ipinfo.io/" + ip));

                    ipInfo = JsonConvert.DeserializeObject<GetIpInfoResponse>(await response.Content.ReadAsStringAsync());

                    RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                    ipInfo.Country = myRI1.EnglishName;
                }
                catch (Exception)
                {
                    ipInfo.Country = null;
                }
            }
        }
        catch (Exception ex)
        {
            ipInfo.Country = null;
        }
        return ipInfo.Country;
    }
}