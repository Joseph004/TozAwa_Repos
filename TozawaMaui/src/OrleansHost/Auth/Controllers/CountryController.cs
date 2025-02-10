using System.Globalization;
using System.Text.Json;
using Grains.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Grains.Auth.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CountryController(ILogger<CountryController> logger) : Controller
    {
        private readonly ILogger<CountryController> _logger = logger;

        [HttpGet, Route("countries")]
        public async Task<ActionResult> GetCountries()
        {
            var cultureList = new Dictionary<string, string>();
            var getCultureInfos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var getCultureInfo in getCultureInfos)
            {
                var getRegionInfo = new RegionInfo(getCultureInfo.Name);
                if (!cultureList.TryGetValue(getRegionInfo.Name, out string value))
                {
                    value = getRegionInfo.NativeName;
                    cultureList.Add(getRegionInfo.Name, value);
                }
            }
            var sortDic = from entry in cultureList orderby entry.Value ascending select entry;
            cultureList = sortDic.ToDictionary();

            return Ok(await Task.FromResult(cultureList));
        }

        [HttpGet, Route("cities")]
        public async Task<ActionResult> GetCities()
        {
            List<City> cities = [];
            try
            {
                Newtonsoft.Json.JsonSerializer serializer = new();
                string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                var assembly = solutiondir + "\\src\\" + "OrleansHost";
                string path = Path.Combine(assembly, @"citysettings.json");

                using FileStream fileStream = new(path, FileMode.Open);
                // Enable case-insensitive deserialization
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                IAsyncEnumerable<City> cityArray = System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<City>(fileStream, options);
                await foreach (City city in cityArray)
                {
                    cities.Add(city);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrive cities {ex}", ex);
            }
            return Ok(cities ?? []);
        }



        [HttpGet, Route("{filter}/{isLimited}")]
        public async Task<ActionResult> GetCountryByCode(string filter, bool isLimited)
        {
            if (string.IsNullOrEmpty(filter))
            {
                Response.StatusCode = 400;
                return Content("Contact support if you see this often");
            }

            var limit = 20;
            var cultureList = new Dictionary<string, string>();
            var getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (var getCulture in getCultureInfo)
            {
                var getRegionInfo = new RegionInfo(getCulture.Name);
                if (!cultureList.TryGetValue(getRegionInfo.Name, out string value))
                {
                    value = getRegionInfo.EnglishName;
                    cultureList.Add(getRegionInfo.Name, value);
                }
            }
            var sortDic = from entry in cultureList orderby entry.Value ascending select entry;
            cultureList = (isLimited ? sortDic.Where(x => !string.IsNullOrEmpty(x.Value) && x.Value.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).Take(limit) :
            sortDic.Where(x => !string.IsNullOrEmpty(x.Value) && x.Value.Contains(filter, StringComparison.InvariantCultureIgnoreCase))).ToDictionary();

            return Ok(await Task.FromResult(cultureList));
        }

        [HttpGet, Route("{countryCode}/{filter}/{isLimited}")]
        public async Task<ActionResult> GetCityByCountry(string countryCode, string filter, bool isLimited)
        {
            if (string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(filter))
            {
                Response.StatusCode = 400;
                return Content("Contact support if you see this often");
            }

            var limit = 20;
            List<City> cities = [];
            try
            {
                Newtonsoft.Json.JsonSerializer serializer = new();
                string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                var assembly = solutiondir + "\\src\\" + "OrleansHost";
                string path = Path.Combine(assembly, @"citysettings.json");

                using FileStream fileStream = new(path, FileMode.Open);
                // Enable case-insensitive deserialization
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                IAsyncEnumerable<City> cityArray = System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<City>(fileStream, options);
                await foreach (City city in cityArray)
                {
                    cities.Add(city);
                }
                cities = isLimited ? [.. cities.Where(x => !string.IsNullOrEmpty(x.Country) && x.Country.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase))
                                 .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).Take(limit)] :
                                   [.. cities.Where(x => !string.IsNullOrEmpty(x.Country) && x.Country.Contains(countryCode, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase))];
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrive cities {ex}", ex);
            }
            return Ok(cities ?? []);
        }
    }
}