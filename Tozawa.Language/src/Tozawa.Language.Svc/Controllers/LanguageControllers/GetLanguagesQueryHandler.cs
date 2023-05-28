using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    public class GetLanguagesQuery : IRequest<IEnumerable<LanguageDto>>
    {
        public bool IncludeDeleted { get; set; }
        public GetLanguagesQuery(bool includeDeleted = false) => IncludeDeleted = includeDeleted;
    }

    public class GetLanguagesQueryHandler : IRequestHandler<GetLanguagesQuery, IEnumerable<LanguageDto>>
    {
        private readonly LanguageContext _context;

        public GetLanguagesQueryHandler(LanguageContext context) => _context = context;

        public async Task<IEnumerable<LanguageDto>> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
        {

            var languages = request.IncludeDeleted
                            ? await _context.Languages.ToListAsync(cancellationToken)
                            : await _context.Languages.Where(x => !x.Deleted).ToListAsync(cancellationToken);

            return languages.OrderBy(x => x.LongName).Select(Result);
        }

        private LanguageDto Result(Languagemd language) => new()
        {
            Id = language.Id,
            Deleted = language.Deleted,
            ShortName = language.ShortName,
            Name = language.Name,
            LongName = language.LongName,
            IsDefault = language.IsDefault
        };
    }
}
