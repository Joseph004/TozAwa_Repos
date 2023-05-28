using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using TozAwaHome.HttpClients;
using TozAwaHome.HttpClients.Helpers;
using TozAwaHome.Models.Dtos;

namespace TozAwaHome.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly IAuthHttpClient _client;
        private readonly ILogger<TranslationService> _logger;
        private readonly ICurrentUserService _currentUserService;

        private ConcurrentDictionary<Guid, string> _translations;
        private ActiveLanguageDto _activeLanguage = null;
        private List<ActiveLanguageDto> _activeLanguages = null;
        private bool _translationLoaded = false;
        private string ActiveLanguageKey => "ActiveLanguageKey";

        public event EventHandler<EventArgs> LanguageChanged;

        public TranslationService(
            IAuthHttpClient client,
            ILogger<TranslationService> logger,
            ICurrentUserService currentUserService)
        {
            _client = client;
            _logger = logger;
            _currentUserService = currentUserService;

            InitTranslations();
        }

        public async Task EnsureTranslations()
        {
            if (!_translationLoaded)
            {
                await LoadTranslations();

                _translationLoaded = true;
            }
        }
        public async Task<string> GetUserCountryByIp()
        {
            /*var ipInfo = new GetIpInfoResponse();
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
            }*/
            return await Task.FromResult("Sweden"); //ipInfo.Country;
        }
        private async Task LoadTranslations()
        {
            var activeLanguage = await GetActiveLanguage();

            var response = await _client.SendGet<Dictionary<Guid, string>>($"translation/systemtexts/{activeLanguage.Id}");

            if (!response.Success || response.Entity == null)
            {
                ClearTranslations();
                return;
            }

            foreach (var item in response.Entity)
            {
                _translations[item.Key] = item.Value;
            }
        }

        private void InitTranslations()
        {
            _translations = new ConcurrentDictionary<Guid, string>();

            foreach (var f in typeof(SystemTextId).GetFields(System.Reflection.BindingFlags.Static))
            {
                var v = (Guid)f.GetValue(null);

                _translations.TryAdd(v, "Not translated");
            }
        }

        private void ClearTranslations()
        {
            foreach (var item in _translations)
            {
                _translations[item.Key] = "Not translated";
            }
        }

        public async Task<ActiveLanguageDto> GetActiveLanguage()
        {
            if (_activeLanguage == null)
            {
                //var currentUser = await _currentUserService.GetCurrentUser();
                var activeLanguageString = await SecureStorage.GetAsync($"{ActiveLanguageKey}_activeLanguage");

                if(!string.IsNullOrEmpty(activeLanguageString))
                {
                  _activeLanguage = JsonConvert.DeserializeObject<ActiveLanguageDto>(activeLanguageString);
                }
            }

            if (_activeLanguage == null)
            {
                _activeLanguage = (await GetActiveLanguages()).FirstOrDefault();
            }

            return _activeLanguage;
        }

        public async Task<List<ActiveLanguageDto>> GetActiveLanguages()
        {
            var country = await GetUserCountryByIp();
            Dictionary<string, string> queryParameters = new();

            if (!string.IsNullOrEmpty(country))
            {
                queryParameters.Add("country", country);
            }

            if (_activeLanguages == null)
            {
                var response = await _client.SendGet<List<ActiveLanguageDto>>($"translation/activelanguages", queryParameters);
                if (!response.Success)
                {
                    if(!string.IsNullOrEmpty(response.Message))
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", response.Message, "OK");
                    }
                    _activeLanguages = new List<ActiveLanguageDto>();
                }
                else
                {
                    _activeLanguages = response.Entity ?? new List<ActiveLanguageDto>();
                }
            }

            return _activeLanguages;
        }

        public async Task ChangeActiveLanguage(Guid languageId)
        {
            if (languageId != _activeLanguage.Id && _activeLanguages.Select(x => x.Id).Contains(languageId))
            {
                //var currentUser = await _currentUserService.GetCurrentUser();
                await SecureStorage.SetAsync($"{ActiveLanguageKey}_activeLanguage", JsonConvert.SerializeObject(_activeLanguages.FirstOrDefault(x => x.Id == languageId)));
                ClearTranslations();
                _activeLanguage = null;
                await LoadTranslations();

                LanguageChanged(this, new EventArgs());
            }
        }

        public string GetShortName(ActiveLanguageDto language)
        {
            if (language == null)
                return "";

            return !string.IsNullOrEmpty(language.ShortName) && language.ShortName.Contains("-") ? language.ShortName.Split("-")[1].ToLower() :
                    !string.IsNullOrEmpty(language.ShortName) && !language.ShortName.Contains("-") ? language.ShortName : "none";
        }

        public TranslationDto Translate(Guid id, string fallback = null, int? limit = null, bool? toUpper = null)
        {
            bool isTranslated = _translations.TryGetValue(id, out string text);

            if (isTranslated
                && (string.IsNullOrEmpty(text) || text.Equals("Not Translated", StringComparison.OrdinalIgnoreCase)))
            {
                isTranslated = false;
            }

            if (!isTranslated)
            {
                text = fallback ?? "Not Translated";
            }

            if (limit.HasValue && isTranslated)
            {
                text = text[..Math.Min(limit.Value, text.Length)];
            }

            if (toUpper.HasValue && toUpper.Value == true && isTranslated)
            {
                text = text.ToUpper();
            }

            return new TranslationDto() { Id = id, Text = text, IsTranslated = isTranslated };
        }

        public async Task<string> GetHttpStatusText(HttpStatusCode? code)
        {
            await EnsureTranslations();

            return code switch
            {
                HttpStatusCode.OK => Translate(SystemTextId.Success).Text,
                HttpStatusCode.Forbidden => Translate(SystemTextId.Forbidden).Text,
                HttpStatusCode.Unauthorized => Translate(SystemTextId.Unauthorized).Text,
                HttpStatusCode.NotFound => Translate(SystemTextId.NotFound).Text,
                _ => Translate(SystemTextId.DefaultError).Text
            };
        }
    }
}
