using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tozawa.Language.Svc.Context;
using Tozawa.Language.Svc.Models;
using MediatR;


namespace Tozawa.Language.Svc.Controllers.SystemTypeControllers
{
    public class GetSystemTypeQuery : IRequest<SystemTypeDto>
    {
        public Guid Id { get; set; }
        public GetSystemTypeQuery(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(GetSystemTypeQuery));
            }
            Id = id;
        }
    }

    public class GetSystemTypeQueryHandler : IRequestHandler<GetSystemTypeQuery, SystemTypeDto>
    {
        private readonly LanguageContext _context;
        public GetSystemTypeQueryHandler(LanguageContext context) => _context = context;

        public async Task<SystemTypeDto> Handle(GetSystemTypeQuery request, CancellationToken cancellationToken)
        {
            var systemType = _context.SystemTypes.FirstOrDefault(x => x.Id == request.Id);
            if (systemType == null)
            {
                throw new ArgumentNullException(nameof(request.Id));
            }
            return await Task.FromResult(Convert(systemType));
        }

        private SystemTypeDto Convert(SystemType systemType) => new SystemTypeDto
        {
            Id = systemType.Id,
            Description = systemType.Description,
            LastUpdated = systemType.LastUpdated
        };
    }
}
