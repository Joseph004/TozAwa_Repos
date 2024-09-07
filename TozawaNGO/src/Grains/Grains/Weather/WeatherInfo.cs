using Orleans.Concurrency;
using System;

namespace Grains
{
    [GenerateSerializer]
    [Immutable]
    public class WeatherInfo(DateTime date, int temperatureC, string summary, int temperatureF)
    {

        [Id(0)]
        public DateTime Date { get; } = date;
        [Id(1)]
        public int TemperatureC { get; } = temperatureC;
        [Id(2)]
        public string Summary { get; } = summary;
        [Id(3)]
        public int TemperatureF { get; } = temperatureF;
    }
}