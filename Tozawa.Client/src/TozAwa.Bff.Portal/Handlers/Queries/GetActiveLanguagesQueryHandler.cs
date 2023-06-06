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
        private readonly IUserCountryByIp _getUserCountryByIp;

        public GetActiveLanguagesQueryHandler(ITranslationService translationService,
        IUserCountryByIp getUserCountryByIp,
            ICurrentUserService currentUserService)
        {
            _translationService = translationService;
            _currentUserService = currentUserService;
            _getUserCountryByIp = getUserCountryByIp;
        }

        public async Task<List<ActiveLanguageDto>> Handle(GetActiveLanguagesQuery request, CancellationToken cancellationToken)
        {
            var country = await _getUserCountryByIp.GetUserCountryByIp();

            var organization = new CurrentUserOrganizationDto();
            var languages = await Task.FromResult(_translationService.GetActiveLanguages());

            return languages.Where(x => x.ShortName.Equals("fr", StringComparison.CurrentCultureIgnoreCase) || x.ShortName.Equals("gb", StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }
}