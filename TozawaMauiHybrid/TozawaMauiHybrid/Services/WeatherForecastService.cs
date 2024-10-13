
using TozawaMauiHybrid.HttpClients;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.ResponseRequests;

namespace TozawaMauiHybrid.Services
{
    public class WeatherForecastService(ITozAwaBffHttpClient client)
    {
        private readonly ITozAwaBffHttpClient _client = client;
        private const string _baseUriPath = $"Weather";

        public async Task<GetResponse<IEnumerable<WeatherDto>>> GetItems()
        {
            var uri = $"{_baseUriPath}/";
            return await _client.SendGet<IEnumerable<WeatherDto>>(uri);
        }
    }
}
