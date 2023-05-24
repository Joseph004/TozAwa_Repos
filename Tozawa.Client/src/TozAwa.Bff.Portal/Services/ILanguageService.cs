using System.Collections.Concurrent;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Services
{
    public interface ILanguageService
    {
        Dictionary<Guid, string> GetTranslations(Guid textId);
        Task<Dictionary<Guid, string>> GetTranslationsAsync(Guid textId);
        Dictionary<Guid, string> GetTranslations(IEnumerable<Guid> languageIds, Guid textId, Guid? systemTypeId = null);
        Task<Dictionary<Guid, string>> GetTranslationsAsync(IEnumerable<Guid> languageIds, Guid textId, Guid? systemTypeId = null);
        ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, string>>> Translations { get; }
        string GetSync(Guid languageId, Guid textId, Guid? systemTypeId = null);
        string GetSync(Guid textId);
        string GetSyncBySystemType(Guid textId, Guid systemTypeId);
        Task<string> Get(Guid languageId, Guid textId, Guid? systemTypeId = null);
        List<ActiveLanguageDto> GetActiveLanguages();
        Task<ImportTranslationResultDto> Import(ImportTranslationDto request);
        Task<string> UpdateText(TranslationUpdateDto request);
        Task<string> UpdateTextBySystemType(TranslationUpdateDto request);
        Task<Guid> AddText(Guid languageId, string text);
        Task<Guid> AddText(string text);
    }
}