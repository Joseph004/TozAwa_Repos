using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Services
{
#nullable enable
    public interface ILanguageText
    {
        Task<Guid?> Add(ILogger logger, object request, List<ImportTranslationTextDto> translations, string text, bool throwErrorOnFail);
        Task<string?> Get(ILogger logger, object request, Guid textId, bool throwErrorOnFail, bool isBySystemType = false);
        Task<string?> UpdateText(ILogger logger, object request, string text, Guid textId, bool throwErrorOnFail);
    }
}