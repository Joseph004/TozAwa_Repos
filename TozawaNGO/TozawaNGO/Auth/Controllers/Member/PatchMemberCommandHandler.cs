

using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Context;
using TozawaNGO.Extension;
using TozawaNGO.Auth.Models.Converters;

namespace TozawaNGO.Auth.Controllers
{
    public class PatchMemberCommandHandler : IRequestHandler<PatchMemberCommand, Models.Dtos.Backend.MemberDto>
    {
        private readonly TozawangoDbContext _context;
        private readonly ILogger<PatchMemberCommandHandler> _logger;

        public PatchMemberCommandHandler(TozawangoDbContext context, ILogger<PatchMemberCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.Dtos.Backend.MemberDto> Handle(PatchMemberCommand request, CancellationToken cancellationToken)
        {
            var member = await _context.TzUsers
                           .FirstOrDefaultAsync(x => x.UserId == request.Id);

            if (member == null)
            {
                _logger.LogWarning("Member not found {id}", request.Id);
                throw new Exception(nameof(request));
            }

            if (request.PatchModel.GetPatchValue<bool>("DeleteForever"))
            {
                _context.TzUsers.Remove(member);
                _context.SaveChanges();
                return MemberConverter.Convert(member, true);
            }

            request.PatchModel.ApplyTo(member);

            _context.SaveChanges();

            return MemberConverter.Convert(member);
        }
    }
}