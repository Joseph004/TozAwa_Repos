using Orleans;
using Grains;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IWeatherGrain : IGrainWithGuidKey
    {
        Task<ImmutableArray<WeatherInfo>> GetForecastAsync();
    }
}