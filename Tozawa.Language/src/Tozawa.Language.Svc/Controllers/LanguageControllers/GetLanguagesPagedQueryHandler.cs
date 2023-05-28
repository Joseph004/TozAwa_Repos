using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tozawa.Language.Svc.Controllers.LanguageControllers
{
    public class GetLanguagesPagedQuery : IRequest<PagedDto<LanguageDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }

        public GetLanguagesPagedQuery(Dictionary<string, StringValues> queryParameters)
        {
            if (queryParameters == null)
            {
                return;
            }

            if (queryParameters.ContainsKey(nameof(SearchString)))
            {
                SearchString = queryParameters[nameof(SearchString)];
            }

            if (queryParameters.ContainsKey(nameof(Page)) && int.TryParse(queryParameters[nameof(Page)], out var page) && page >= 1)
            {
                Page = page;
            }

            if (queryParameters.ContainsKey(nameof(PageSize)) && int.TryParse(queryParameters[nameof(PageSize)], out var pageSize) && pageSize >= 1)
            {
                PageSize = pageSize;
            }
        }
    }

    public class GetLanguagesPagedQueryHandler : IRequestHandler<GetLanguagesPagedQuery, PagedDto<LanguageDto>>
    {
        private readonly LanguageContext _context;
        public GetLanguagesPagedQueryHandler(LanguageContext context) => _context = context;

        public async Task<PagedDto<LanguageDto>> Handle(GetLanguagesPagedQuery request, CancellationToken cancellationToken)
        {
            List<Languagemd> languages;
            int totalItems = 0;
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                languages = await _context.Languages.Where(x => x.LongName.Contains(request.SearchString) || x.ShortName.Contains(request.SearchString))
                                                        .OrderBy(x => x.LongName)
                                                        .Skip((request.Page - 1) * request.PageSize)
                                                        .Take(request.PageSize)
                                                        .ToListAsync(cancellationToken);
                totalItems = await _context.Languages.Where(x => x.LongName.Contains(request.SearchString))
                                                       .CountAsync(cancellationToken);
            }
            else
            {
                languages = await _context.Languages.OrderBy(x => x.LongName)
                                                        .Skip((request.Page - 1) * request.PageSize)
                                                        .Take(request.PageSize)
                                                        .ToListAsync(cancellationToken);
                totalItems = await _context.Languages.CountAsync(cancellationToken);
            }

            return new PagedDto<LanguageDto>
            {
                Page = request.Page,
                TotalItems = totalItems,
                PageSize = request.PageSize,
                Items = languages.Select(Convert)
            };
        }

        private LanguageDto Convert(Languagemd language) => new()
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
