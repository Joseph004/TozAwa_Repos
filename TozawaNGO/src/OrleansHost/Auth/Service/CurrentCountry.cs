

using System.Globalization;
using IpInfo.Api.Client.Models;
using Newtonsoft.Json;

namespace OrleansHost.Auth.Services;

public class CurrentCountry : ICurrentCountry
{
    public async Task<IPCountryResponse> GetUserCountryByIp()
    {
        var ipInfo = new GetIpInfoResponse();
        var ip = "";
        try
        {
            var httpClient = new HttpClient();
            var ipResponse = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, "https://ipv4.icanhazip.com/"));

            var jsonString = await ipResponse.Content.ReadAsStringAsync();

            ip = jsonString.Replace("\n", "").Replace("\r", "");
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
        catch (Exception)
        {
            ipInfo.Country = null;
        }
        return new IPCountryResponse
        {
            Country = ipInfo.Country,
            State = ipInfo.State,
            City = ipInfo.City,
            Ip = ip
        };
    }
}

public class IPCountryResponse
{
    public string Country { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Ip { get; set; }
}