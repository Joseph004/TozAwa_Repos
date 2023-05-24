

using MediatR;
using Microsoft.EntityFrameworkCore;
using Tozawa.Auth.Svc.Context;
using Tozawa.Auth.Svc.extension;
using Tozawa.Auth.Svc.Models.Converters;

namespace Tozawa.Auth.Svc.Controllers
{
    public class PatchMemberCommandHandler : IRequestHandler<PatchMemberCommand, Models.Dtos.Backend.MemberDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatchMemberCommandHandler> _logger;

        public PatchMemberCommandHandler(ApplicationDbContext context, ILogger<PatchMemberCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.Dtos.Backend.MemberDto> Handle(PatchMemberCommand request, CancellationToken cancellationToken)
        {
            var member = await _context.Users
                           .FirstOrDefaultAsync(x => x.UserId == request.Id);

            if (member == null)
            {
                _logger.LogWarning("Member not found {id}", request.Id);
                throw new Exception(nameof(request));
            }

            if (request.PatchModel.GetPatchValue<bool>("DeleteForever"))
            {
                _context.Users.Remove(member);
                _context.SaveChanges();
                return MemberConverter.Convert(member, true);
            }

            request.PatchModel.ApplyTo(member);

            _context.SaveChanges();

            return MemberConverter.Convert(member);
        }
    }
}