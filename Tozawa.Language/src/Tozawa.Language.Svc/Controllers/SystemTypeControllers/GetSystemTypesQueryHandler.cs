using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using MediatR;

namespace Tozawa.Language.Svc.Controllers.SystemTypeControllers
{
    public class GetSystemTypesQuery : IRequest<IEnumerable<SystemTypeDto>>
    {
    }

    public class GetSystemTypesQueryHandler : IRequestHandler<GetSystemTypesQuery, IEnumerable<SystemTypeDto>>
    {
        private readonly LanguageContext _context;
        public GetSystemTypesQueryHandler(LanguageContext context) => _context = context;

        public async Task<IEnumerable<SystemTypeDto>> Handle(GetSystemTypesQuery request, CancellationToken cancellationToken)
        {
            var systemTypes = _context.SystemTypes.OrderBy(x => x.Description).ToList();
            return await Task.FromResult(systemTypes.Select(Convert));
        }

        private SystemTypeDto Convert(SystemType systemType) => new SystemTypeDto
        {
            Id = systemType.Id,
            Description = systemType.Description,
            LastUpdated = systemType.LastUpdated,
            IsDefault = systemType.IsDefault
        };
    }
}
