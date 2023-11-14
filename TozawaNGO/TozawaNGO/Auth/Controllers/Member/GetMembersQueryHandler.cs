
using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Context;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Converters;

namespace TozawaNGO.Auth.Controllers
{
    public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, IEnumerable<Models.Dtos.Backend.MemberDto>>
    {
        private readonly TozawangoDbContext _context;

        public GetMembersQueryHandler(TozawangoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Dtos.Backend.MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ApplicationUser> members = _context.TzUsers
                .Include(x => x.Partner);

            var result = await members
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return MemberConverter.Convert(result);
        }
    }
}