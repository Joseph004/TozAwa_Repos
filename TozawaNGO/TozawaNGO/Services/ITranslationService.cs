using System.Net;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Services;

public interface ITranslationService
{
    event EventHandler<EventArgs> LanguageChanged;
    Task EnsureTranslations();
    TranslationDto Translate(Guid id, string fallback = null, int? limit = null, bool? toUpper = null);
    Task<List<ActiveLanguageDto>> GetActiveLanguages();
    //Task<ActiveLanguageDto> GetActiveLanguageFirst();
    Task<ActiveLanguageDto> GetActiveLanguage();
    Task ChangeActiveLanguage(Guid languageId);
    string GetShortName(ActiveLanguageDto language);
    Task<string> GetHttpStatusText(HttpStatusCode? code);
}
