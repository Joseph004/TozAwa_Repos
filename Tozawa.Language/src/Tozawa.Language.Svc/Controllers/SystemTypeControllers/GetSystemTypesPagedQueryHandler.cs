using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Tozawa.Language.Svc.Controllers.SystemTypeControllers
{
    public class GetSystemTypesPagedQuery : IRequest<PagedDto<SystemTypeDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }

        public GetSystemTypesPagedQuery(Dictionary<string, StringValues> queryParameters)
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

    public class GetSystemTypesPagedQueryHandler : IRequestHandler<GetSystemTypesPagedQuery, PagedDto<SystemTypeDto>>
    {
        private readonly LanguageContext _context;
        public GetSystemTypesPagedQueryHandler(LanguageContext context) => _context = context;

        public async Task<PagedDto<SystemTypeDto>> Handle(GetSystemTypesPagedQuery request, CancellationToken cancellationToken)
        {
            List<SystemType> systemTypes;
            int totalItems = 0;
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                systemTypes = await _context.SystemTypes.Where(x => x.Description.Contains(request.SearchString))
                                                        .OrderBy(x => x.Description)
                                                        .Skip((request.Page - 1) * request.PageSize)
                                                        .Take(request.PageSize)
                                                        .ToListAsync(cancellationToken);
                totalItems = await _context.SystemTypes.Where(x => x.Description.Contains(request.SearchString))
                                                       .CountAsync(cancellationToken);
            }
            else
            {
                systemTypes = await _context.SystemTypes.OrderBy(x => x.Description)
                                                        .Skip((request.Page - 1) * request.PageSize)
                                                        .Take(request.PageSize)
                                                        .ToListAsync(cancellationToken);
                totalItems = await _context.SystemTypes.CountAsync(cancellationToken);
            }

            return new PagedDto<SystemTypeDto>
            {
                Page = request.Page,
                TotalItems = totalItems,
                PageSize = request.PageSize,
                Items = systemTypes.Select(Convert)
            };
        }

        private SystemTypeDto Convert(SystemType systemType) => new()
        {
            Id = systemType.Id,
            Description = systemType.Description,
            LastUpdated = systemType.LastUpdated,
            IsDefault = systemType.IsDefault
        };
    }
}