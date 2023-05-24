using System.Net;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Client.Portal.HttpClients;

namespace Tozawa.Bff.Portal.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ILanguageService _LanguageService;
        private readonly ILanguageHttpClient _LanguageHttpClient;

        public TranslationService(ILanguageService LanguageService, ILanguageHttpClient LanguageHttpClient)
        {
            _LanguageService = LanguageService;
            _LanguageHttpClient = LanguageHttpClient;
        }

        public async Task<Dictionary<Guid, string>> GetSystemTexts(Guid languageId, Guid systemTypeId)
        {
            var systemTexts = await _LanguageHttpClient.GetTranslations(languageId, systemTypeId);
            if (systemTexts == null)
            {
                return new Dictionary<Guid, string>();
            }

            return systemTexts.ToDictionary(x => x.Id, x => x.Text);
        }

        public List<ActiveLanguageDto> GetActiveLanguages()
        {
            var activeLanguages = _LanguageService.GetActiveLanguages();

            if (activeLanguages == null)
            {
                return new List<ActiveLanguageDto>();
            }

            return activeLanguages;
        }
        public async Task<string> GetHttpStatusText(HttpStatusCode? code)
        {
            return code switch
            {
                HttpStatusCode.OK => UpdateMessages.Success,
                HttpStatusCode.Forbidden => UpdateMessages.Forbidden,
                HttpStatusCode.Unauthorized => UpdateMessages.Unauthorized,
                HttpStatusCode.NotFound => UpdateMessages.NotFound,
                _ => UpdateMessages.Error
            };
        }
    }
}
