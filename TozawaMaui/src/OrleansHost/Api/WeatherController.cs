using Microsoft.AspNetCore.Mvc;
using Grains;
using System.Collections.Immutable;
using Asp.Versioning;

namespace OrleansHost.Api
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/[controller]")]
    public class WeatherController(IGrainFactory factory) : ControllerBase
    {
        private readonly IGrainFactory factory = factory;

        [HttpGet]
        public Task<ImmutableArray<WeatherInfo>> GetAsync() =>
            factory.GetGrain<IWeatherGrain>(Guid.Empty).GetForecastAsync();
    }
}