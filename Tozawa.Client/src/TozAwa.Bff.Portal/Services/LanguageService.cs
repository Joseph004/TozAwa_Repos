using System.Collections.Concurrent;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Client.Portal.HttpClients;

namespace Tozawa.Bff.Portal.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILogger<LanguageService> _logger;
        private readonly ILanguageHttpClient _LanguageHttpClient;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppSettings _appSettings;
        private const string NotTranslated = "Not Translated";
        public ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>> Translations { get; } = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>>();
        public LanguageService(ILanguageHttpClient LanguageHttpClient, AppSettings appSettings, ICurrentUserService currentUserService, ILogger<LanguageService> logger)
        {
            _logger = logger;
            _LanguageHttpClient = LanguageHttpClient;
            _currentUserService = currentUserService;
            _appSettings = appSettings;
        }

        public Dictionary<Guid, string> GetTranslations(Guid textId)
        {
            var task = GetTranslationsAsync(textId);
            task.Wait();
            return task.Result;
        }

        public async Task<Dictionary<Guid, string>> GetTranslationsAsync(Guid textId)
        {
            var result = new Dictionary<Guid, string>();

            var languageIds = _currentUserService.User?.OrganizationLanguageIds ?? null;
            if (languageIds == null)
            {
                languageIds = new List<Guid> { _currentUserService.LanguageId };
            }
            foreach (var language in languageIds)
            {
                var translation = await Get(language, textId, _appSettings.SystemTextGuid);
                result.TryAdd(language, translation);
            }
            return result;
        }

        public Dictionary<Guid, string> GetTranslations(IEnumerable<Guid> languageIds, Guid textId, Guid? systemTypeId = null)
        {
            var task = GetTranslationsAsync(languageIds, textId, systemTypeId);
            task.Wait();
            return task.Result;
        }

        public async Task<Dictionary<Guid, string>> GetTranslationsAsync(IEnumerable<Guid> languageIds, Guid textId, Guid? systemTypeId = null)
        {
            var result = new Dictionary<Guid, string>();
            if (languageIds == null) return result;
            foreach (var language in languageIds)
            {
                var translation = await Get(language, textId, systemTypeId);
                result.TryAdd(language, translation);
            }
            return result;
        }

        public string GetSync(Guid languageId, Guid textId, Guid? systemTypeId = null)
        {
            var task = Get(languageId, textId, systemTypeId);
            task.Wait();
            return task.Result;
        }

        public List<ActiveLanguageDto> GetActiveLanguages()
        {
            if (LanguageTranslations.Languages.Any())
            {
                return LanguageTranslations.Languages;
            }
            var task = _LanguageHttpClient.GetActiveLanguages();
            task.Wait();
            LanguageTranslations.Languages = task.Result;
            return LanguageTranslations.Languages;
        }

        public string GetSync(Guid textId) => GetSync(_currentUserService.LanguageId, textId, _appSettings.SystemTextGuid);
        public string GetSyncBySystemType(Guid textId, Guid systemTypeId) => GetSync(_currentUserService.LanguageId, textId, systemTypeId);

        public async Task<string> Get(Guid languageId, Guid textId, Guid? systemTypeId = null)
        {
            var id = systemTypeId ?? _appSettings.SystemTextGuid;
            if (IsCachedInService(id, languageId))
            {
                return Return(languageId, textId, id);
            }
            if (await IsCached(id, languageId))
            {
                if (!Translations.ContainsKey(id))
                {
                    Translations.TryAdd(id, new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>());
                }
                if (Translations.ContainsKey(id))
                {
                    Translations[id][languageId] = LanguageTranslations.Translations[id][languageId];
                }
                return Return(languageId, textId, id);
            }
            await FetchAndCacheTranslations(languageId, id);
            return Return(languageId, textId, id);
        }

        public async Task<bool> IsCached(Guid systemTypeId, Guid languageId)
        {
            try
            {
                var systemType = await _LanguageHttpClient.GetCacheTimeStamp(systemTypeId);
                return systemType != null && IsCachedInLanguageTranslations(systemTypeId, languageId, systemType.LastUpdated);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "IsCached Unable to get cache timestamp for {systemTypeId} languageid {languageId} trying to use last cached texts", systemTypeId, languageId);
                return LanguageTranslations.IsCached(systemTypeId, languageId);
            }
        }

        private string Return(Guid languageId, Guid textId, Guid systemTypeId)
        {
            if (Translations[systemTypeId][languageId].ContainsKey(textId) && !string.IsNullOrEmpty(Translations[systemTypeId][languageId][textId]))
            {
                return Translations[systemTypeId][languageId][textId];
            }

            if (_currentUserService.User.FallbackLanguageId.HasValue && languageId != _currentUserService.User.FallbackLanguageId.Value)
            {
                return GetSync(_currentUserService.User.FallbackLanguageId.Value, textId, systemTypeId);
            }

            return NotTranslated;
        }

        private async Task FetchAndCacheTranslations(Guid languageId, Guid systemTypeId)
        {
            var translations = await _LanguageHttpClient.GetTranslations(languageId, systemTypeId);
            if (!Translations.ContainsKey(systemTypeId))
            {
                Translations.TryAdd(systemTypeId, new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>());
            }
            if (!translations.Any())
            {
                Translations[systemTypeId][languageId] = new ConcurrentDictionary<Guid, string>();
                SystemTypeDto systemType;
                try
                {
                    systemType = await _LanguageHttpClient.GetCacheTimeStamp(systemTypeId);
                }
                catch (Exception e)
                {
                    systemType = null;
                    _logger.LogError(e, "FetchAndCacheTranslations Unable to get cache timestamp for {systemTypeId} languageid {languageId} setting cachetimestamp to datetime.minvalue", systemType, languageId);
                }

                LanguageTranslations.Set(systemTypeId, languageId, systemType?.LastUpdated ?? DateTime.MinValue, new Dictionary<Guid, string>());
            }
            else
            {
                var transformedTranslations = TransformTranslationsListToDictionary(translations);
                SystemTypeDto systemType;
                try
                {
                    systemType = await _LanguageHttpClient.GetCacheTimeStamp(systemTypeId);
                }
                catch (Exception e)
                {
                    systemType = null;
                    _logger.LogError(e, "FetchAndCacheTranslations Unable to get cache timestamp for {systemTypeId} languageid {languageId} setting cachetimestamp to datetime.minvalue", systemType, languageId);
                }
                Translations[systemTypeId][languageId] = new ConcurrentDictionary<Guid, string>(transformedTranslations);
                LanguageTranslations.Set(systemTypeId, languageId, systemType?.LastUpdated ?? DateTime.MinValue, transformedTranslations);
            }
        }

        private static bool IsCachedInLanguageTranslations(Guid systemtypeId, Guid languageId, DateTime cachedTimeStamp) => LanguageTranslations.IsCached(systemtypeId, languageId, cachedTimeStamp);
        private bool IsCachedInService(Guid systemTypeId, Guid languageId) => Translations.ContainsKey(systemTypeId) && Translations[systemTypeId].ContainsKey(languageId);
        private static Dictionary<Guid, string> TransformTranslationsListToDictionary(IEnumerable<TranslationDto> translations) => translations.ToDictionary(x => x.Id, x => x.Text);

        public async Task<ImportTranslationResultDto> Import(ImportTranslationDto request)
        {
            var result = await _LanguageHttpClient.Import(request);

            AddToCache(request.Original.LanguageId, result.TextId, request.Original.Text);
            if (request.Translations != null)
            {
                foreach (var translation in request.Translations.Where(t => !string.IsNullOrEmpty(t.Text)))
                {
                    AddToCache(translation.LanguageId, result.TextId, translation.Text);
                }
            }

            return result;
        }

        public async Task<string> UpdateText(TranslationUpdateDto request)
        {
            var result = await _LanguageHttpClient.UpdateText(request);
            AddToCache(request.LanguageId, request.Id, request.Text);
            return result;
        }

        public async Task<string> UpdateTextBySystemType(TranslationUpdateDto request)
        {
            var result = await _LanguageHttpClient.UpdateTextBySystemType(request);
            AddToCache(request.LanguageId, request.Id, request.Text);
            return result;
        }

        private void AddToCache(Guid languageId, Guid textId, string text)
        {
            if (IsCachedInService(_appSettings.SystemTextGuid, languageId))
            {
                Translations[_appSettings.SystemTextGuid][languageId][textId] = text;
            }
        }

        public async Task<Guid> AddText(Guid languageId, string text) => await _LanguageHttpClient.AddTranslation(new TranslationAddDto
        {
            LanguageId = languageId,
            Text = text,
            SystemTypeId = _appSettings.SystemTextGuid
        });

        public async Task<Guid> AddText(string text) => await _LanguageHttpClient.AddTranslation(new TranslationAddDto
        {
            LanguageId = _currentUserService.LanguageId,
            Text = text,
            SystemTypeId = _appSettings.SystemTextGuid
        });
    }
}