using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TozawaNGO.HttpClients;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using Blazored.SessionStorage;
using TozawaNGO.Configurations;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;
using System.Resources;
using System.Collections;
using Humanizer.Localisation;
using System.Xml.Linq;
using System.Reflection;

namespace TozawaNGO.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ILogger<TranslationService> _logger;
        private readonly ILocalStorageService _localStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;

        private ConcurrentDictionary<Guid, string> _translations;
        private ActiveLanguageDto _activeLanguage = null;
        private List<ActiveLanguageDto> _activeLanguages = null;
        private bool _translationLoaded = false;
        private string ActiveLanguageKey => "ActiveLanguageKey";
        private CultureInfo _selectedCulture = Thread.CurrentThread.CurrentCulture;

        public event EventHandler<EventArgs> LanguageChanged;

        public TranslationService(
            ILogger<TranslationService> logger,
            IHttpContextAccessor httpContextAccessor,
            ILocalStorageService localStorageService,
            AppSettings appSettings)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _localStorageService = localStorageService;
            _appSettings = appSettings;

            InitTranslations();
        }

        public async Task EnsureTranslations()
        {
            if (!_translationLoaded)
            {
                await LoadTranslations();

                _translationLoaded = true;

                LanguageChanged(this, new EventArgs());
            }
        }
       /*  public async Task AddToResource(string text, Guid textId, Guid languageId)
        {
            var languages = await GetActiveLanguages();
            var language = languages.First(x => x.Id == languageId);
            var languageShortName = language.Culture;

            var path = AppDomain.CurrentDomain.BaseDirectory + "Resources\\App.resx";
            if (languageShortName == "fr")
            {
                path = AppDomain.CurrentDomain.BaseDirectory + "Resources\\App.fr.resx";
            }
            else if (languageShortName == "sv")
            {
                path = AppDomain.CurrentDomain.BaseDirectory + "Resources\\App.sv.resx";
            }

            using FileStream stream = File.Open(path, FileMode.Open);
            XDocument doc = XDocument.Load(stream);

            XElement data = new("data");

            XNamespace ns = "xml";
            data.Add(new XAttribute("name", textId.ToString()));
            data.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));

            data.Add(new XElement("value", text));
            data.Add(new XElement("comment", "."));

            doc.Element("root").Add(data);
            doc.Save(stream);
            stream.Close();
        } */
        private async IAsyncEnumerable<LocalizedString> GetAllLocalizedStrings()
        {
            var activeCulture = await GetActiveLanguage();

            if (activeCulture != null && activeCulture.Culture != _selectedCulture.Name)
            {
                var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                var culture = cultures.FirstOrDefault(x => x.Name == activeCulture.Culture);

                if (culture != null)
                {
                    _selectedCulture = culture;
                }
            }

            ResourceManager rm = new(typeof(TozawaNGO.Resources.App));
            foreach (DictionaryEntry value in rm.GetResourceSet(_selectedCulture, true, true))
            {
                yield return new LocalizedString((string)value.Key, (string)value.Value);
            }
        }
        private async Task LoadTranslations()
        {
            var allStrings = GetAllLocalizedStrings();

            var translations = await allStrings.ToListAsync();

            if (translations == null)
            {
                ClearTranslations();
                return;
            }
            foreach (var translation in translations)
            {
                var isGuid = Guid.TryParse(translation.Name, out Guid result);

                if (isGuid)
                {
                    if (translation.Name == translation.Value)
                    {
                        _translations[result] = "Not Translated";
                    }
                    else
                    {
                        _translations[result] = translation.Value;
                    }
                }
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

        public async Task<ActiveLanguageDto> GetActiveLanguageFirst()
        {
            var languages = await GetActiveLanguages();

            if (!string.IsNullOrEmpty(_selectedCulture.Name))
            {
                _activeLanguage = languages.FirstOrDefault(x => x.Culture.Equals(_selectedCulture.Name, StringComparison.CurrentCultureIgnoreCase));

                if (_activeLanguage != null)
                {
                    return _activeLanguage;
                }
            }

            if (_activeLanguage == null)
            {
                _activeLanguage ??= languages.FirstOrDefault();
            }

            return _activeLanguage;
        }
        public async Task<ActiveLanguageDto> GetActiveLanguage()
        {
            var languages = await GetActiveLanguages();

            _activeLanguage ??= _localStorageService != null ? await _localStorageService.GetItemAsync<ActiveLanguageDto>($"{ActiveLanguageKey}_activeLanguage") : null;
            _activeLanguage ??= languages.FirstOrDefault();

            return _activeLanguage;
        }

        public async Task<List<ActiveLanguageDto>> GetActiveLanguages()
        {
            Dictionary<string, string> queryParameters = new();

            if (_activeLanguages == null)
            {
                _activeLanguages = (_appSettings.Languages ?? Array.Empty<ActiveLanguageDto>()).ToList();
            }

            return await Task.FromResult(_activeLanguages);
        }

        public async Task ChangeActiveLanguage(Guid languageId)
        {
            if (languageId != _activeLanguage.Id && _activeLanguages.Select(x => x.Id).Contains(languageId))
            {
                //var currentUser = await _currentUserService.GetCurrentUser();
                await _localStorageService.SetItemAsync($"{ActiveLanguageKey}_activeLanguage", _activeLanguages.FirstOrDefault(x => x.Id == languageId));
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
