

using System;
using System.Collections.Concurrent;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Models
{
    public static class LanguageTranslations
    {
        public static ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>> Translations { get; } = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>>();
        public static ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, DateTime>> CacheTimeStamps { get; } = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, DateTime>>();

        public static List<ActiveLanguageDto> Languages { get; set; } = new List<ActiveLanguageDto>();
        public static void Set(Guid systemTypeId, Guid languageId, DateTime lastUpdated, Dictionary<Guid, string> transformedTranslations)
        {
            if (!Translations.ContainsKey(systemTypeId))
            {
                Translations[systemTypeId] = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>();
            }
            if (Translations[systemTypeId].ContainsKey(languageId))
            {
                Translations[systemTypeId].TryRemove(languageId, out var _);
            }

            if (!CacheTimeStamps.ContainsKey(systemTypeId))
            {
                CacheTimeStamps[systemTypeId] = new ConcurrentDictionary<Guid, DateTime>();
            }
            if (CacheTimeStamps[systemTypeId].ContainsKey(languageId))
            {
                CacheTimeStamps[systemTypeId].TryRemove(languageId, out var _);
            }
            if (Translations[systemTypeId].TryAdd(languageId, new ConcurrentDictionary<Guid, string>(transformedTranslations)))
            {
                CacheTimeStamps[systemTypeId].TryAdd(languageId, lastUpdated);
            }
        }

        public static bool IsCached(Guid systemTypeId, Guid languageId, DateTime lastUpdated)
        => CacheTimeStamps.ContainsKey(systemTypeId) &&
            CacheTimeStamps[systemTypeId].ContainsKey(languageId) &&
            CacheTimeStamps[systemTypeId][languageId] == lastUpdated;

        public static bool IsCached(Guid systemTypeId, Guid languageId)
        => CacheTimeStamps.ContainsKey(systemTypeId) && CacheTimeStamps[systemTypeId].ContainsKey(languageId);
    }
}
