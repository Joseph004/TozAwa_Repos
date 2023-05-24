using Newtonsoft.Json;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Services
{
#nullable enable
    public class LanguageText : ILanguageText
    {
        private readonly ILanguageService _languageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppSettings _appsettings;

        public LanguageText(ILanguageService languageService, ICurrentUserService currentUserService, AppSettings appsettings)
        {
            _languageService = languageService;
            _currentUserService = currentUserService;
            _appsettings = appsettings;
        }
        private Guid _organizationId => _currentUserService.User.OrganizationId;
        private Guid _languageId => _currentUserService.LanguageId == Guid.Empty ? Guid.Parse("ea3ff133-cd1b-42d7-9d42-0aeee6b731c8") : _currentUserService.LanguageId;
        public async Task<string?> Get(ILogger logger, object request, Guid textId, bool throwErrorOnFail, bool isBySystemType = false)
        {
            var LanguageText = "Not Translated";
            if (isBySystemType)
            {
                LanguageText = _languageService.GetSyncBySystemType(textId, _appsettings.SystemTextGuid);
            }
            else
            {
                LanguageText = await _languageService.Get(_languageId, textId, _organizationId);
            }

            if (LanguageText == null)
            {
                logger.LogError("Failed to get text {text} for request {req} organizationid {orgid}", textId.ToString(), JsonConvert.SerializeObject(request), _organizationId);
                if (throwErrorOnFail)
                {
                    throw new ArgumentException("Failed to get text");
                }
                return null;
            }
            return LanguageText;
        }
        public async Task<string?> UpdateText(ILogger logger, object request, string text, Guid textId, bool throwErrorOnFail)
        {
            if (string.IsNullOrEmpty(text) && textId == Guid.Empty)
            {
                if (throwErrorOnFail)
                {
                    logger.LogError("Text missing for request {req} organizationid {orgid}", JsonConvert.SerializeObject(request), _organizationId);
                    throw new ArgumentException("Required text missing");
                }
                return null;
            }
            var LanguageText = await _languageService.UpdateText(new TranslationUpdateDto
            {
                Id = textId,
                LanguageId = _languageId,
                Text = text,
                SystemTypeId = _organizationId
            });
            if (LanguageText == null)
            {
                logger.LogError("Failed to update text {text} for request {req} organizationid {orgid}", text, JsonConvert.SerializeObject(request), _organizationId);
                if (throwErrorOnFail)
                {
                    throw new ArgumentException("Failed to upadte text");
                }
                return null;
            }
            return LanguageText;
        }
        public async Task<Guid?> Add(ILogger logger, object request, List<ImportTranslationTextDto> translations, string text, bool throwErrorOnFail)
        {
            return await Add(logger, request, translations, _organizationId, _languageId, text, throwErrorOnFail);
        }
        public async Task<Guid?> Add(ILogger logger, object request, List<ImportTranslationTextDto> translations, Guid organizationId, Guid languageId, string text, bool throwErrorOnFail)
        {
            var LanguageText = new ImportTranslationResultDto();
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    if (throwErrorOnFail)
                    {
                        logger.LogError("Text missing for request {req} organizationid {orgid}", JsonConvert.SerializeObject(request), organizationId);
                        throw new ArgumentException("Required text missing");
                    }
                    return null;
                }

                LanguageText = await _languageService.Import(new ImportTranslationDto { SystemTypeId = organizationId, Original = new ImportTranslationTextDto { LanguageId = languageId, Text = text }, Translations = translations ?? new List<ImportTranslationTextDto>() });


                if (LanguageText == null)
                {
                    logger.LogError("Failed to create text {text} for request {req} organizationid {orgid}", text, JsonConvert.SerializeObject(request), organizationId);
                    if (throwErrorOnFail)
                    {
                        throw new ArgumentException("Failed to create text");
                    }
                    return null;
                }

            }
            catch
            {
                logger.LogError("Failed to create text {text} for request {req} organizationid {orgid}", text, JsonConvert.SerializeObject(request), organizationId);
                throw new ArgumentException("Failed to create text");
            }
            return LanguageText.TextId;
        }
    }
}