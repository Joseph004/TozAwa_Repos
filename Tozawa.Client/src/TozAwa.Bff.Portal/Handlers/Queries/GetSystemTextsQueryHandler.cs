using MediatR;
using Tozawa.Bff.Portal.Configuration;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Handlers.Queries
{
    public class GetSystemTextsQueryHandler : IRequestHandler<GetSystemTextsQuery, Dictionary<Guid, string>>
    {
        private readonly ITranslationService _translationService;
        private readonly AppSettings _appSettings;

        public GetSystemTextsQueryHandler(ITranslationService translationService, AppSettings appSettings)
        {
            _translationService = translationService;
            _appSettings = appSettings;
        }

        public async Task<Dictionary<Guid, string>> Handle(GetSystemTextsQuery request, CancellationToken cancellationToken)
        => await _translationService.GetSystemTexts(request.LanguageId, _appSettings.SystemTextGuid);
    }
}