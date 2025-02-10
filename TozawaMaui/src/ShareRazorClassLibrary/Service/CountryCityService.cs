using Microsoft.Extensions.Logging;
using ShareRazorClassLibrary.HttpClients;
using ShareRazorClassLibrary.Models;

namespace ShareRazorClassLibrary.Services;

public interface ICountryCityService
{
    Task<List<Country>> GetCountryByCode(string filter = null);
    Task<List<City>> GetCityByCountry(string countryCode, string filter = null);
    void RemoveData();
}
public class CountryCityService(ITozAwaBffHttpClient client, ILogger<CountryCityService> logger) : ICountryCityService
{
    private readonly ITozAwaBffHttpClient _client = client;
    private readonly ILogger<CountryCityService> _logger = logger;
    private List<City> _cities { get; set; } = [];
    private List<Country> _countries { get; set; } = [];
    private const string _baseUriPath = $"country";

    public async Task<List<Country>> GetCountryByCode(string filter = null)
    {
        var limit = 15;
        List<Country> counties = [];
        try
        {
            if (_countries.Count == 0)
            {
                var response = await _client.SendGet<Dictionary<string, string>>($"{_baseUriPath}/countries");
                foreach (var item in response.Entity ?? [])
                {
                    if (_countries.Any(x => x.Code == item.Key)) continue;
                    _countries.Add(new Country
                    {
                        Name = item.Value,
                        Code = item.Key
                    });
                }
            }
            _countries = [.. _countries.DistinctBy(x => x.Code)];
            counties = filter == null ? [.. _countries.Take(limit)] :
                [.. _countries.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase))];
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to retrive countries {ex}", ex);
        }
        return counties;
    }
    public async Task<List<City>> GetCityByCountry(string countryCode, string filter = null)
    {
        var limit = 15;
        List<City> cities = [];

        try
        {
            if (_cities.Count == 0)
            {
                var response = await _client.SendGet<List<City>>($"{_baseUriPath}/cities");
                _cities = response.Entity ?? [];
            }

            cities = filter == null ? [.. _cities.Where(x => !string.IsNullOrEmpty(x.Country) && x.Country.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase)).Take(limit)] :
                                   [.. _cities.Where(x => !string.IsNullOrEmpty(x.Country) && x.Country.Contains(countryCode, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase))];
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to retrive cities {ex}", ex);
        }

        return cities;
    }
    public void RemoveData()
    {
        _countries = [];
        _cities = [];
    }
}

