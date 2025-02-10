using System.Net;
using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Services;

public interface ITranslationService
{
    event EventHandler<EventArgs> LanguageChanged;
    Task EnsureTranslations();
    TranslationDto Translate(Guid id, string fallback = null, int? limit = null, bool? toUpper = null);
    Task<List<ActiveLanguageDto>> GetActiveLanguages();
    bool TranslationLoaded();
    Task<ActiveLanguageDto> GetActiveLanguage();
    Task ChangeActiveLanguage(Guid languageId);
    string GetShortName(ActiveLanguageDto language);
    ActiveLanguageDto ActiveLanguage();
    Task<string> GetHttpStatusText(HttpStatusCode code);
}
