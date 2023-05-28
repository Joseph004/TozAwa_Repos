using MediatR;
using Tozawa.Bff.Portal.Models;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Handlers.Queries
{
    public class GetActiveLanguagesQueryHandler : IRequestHandler<GetActiveLanguagesQuery, List<ActiveLanguageDto>>
    {
        private readonly ITranslationService _translationService;
        private readonly ICurrentUserService _currentUserService;

        public GetActiveLanguagesQueryHandler(ITranslationService translationService,
            ICurrentUserService currentUserService)
        {
            _translationService = translationService;
            _currentUserService = currentUserService;
        }

        public async Task<List<ActiveLanguageDto>> Handle(GetActiveLanguagesQuery request, CancellationToken cancellationToken)
        {
            var organization = new CurrentUserOrganizationDto();
            var languages = await Task.FromResult(_translationService.GetActiveLanguages());

            return languages.Where(x => x.ShortName.Equals("fr", StringComparison.CurrentCultureIgnoreCase) || x.ShortName.Equals("gb", StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }
}