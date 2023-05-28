
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Auth.Svc.Context;
using Tozawa.Auth.Svc.Models.Authentication;
using Tozawa.Auth.Svc.Models.Converters;

namespace Tozawa.Auth.Svc.Controllers
{
    public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, IEnumerable<Models.Dtos.Backend.MemberDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetMembersQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Dtos.Backend.MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ApplicationUser> members = _context.Users
                .Include(x => x.Roles).ThenInclude(x => x.Role.Functions)
                .Include(x => x.Organizations).ThenInclude(x => x.PartnerOrganizationsFrom)
                .Include(x => x.Organizations).ThenInclude(x => x.PartnerOrganizationsTo)
                .Include(x => x.Organizations).ThenInclude(x => x.Features);

            var result = await members
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return MemberConverter.Convert(result);
        }
    }
}