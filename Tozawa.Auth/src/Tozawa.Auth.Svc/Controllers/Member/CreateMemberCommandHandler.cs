
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tozawa.Auth.Svc.Context;
using Tozawa.Auth.Svc.Models.Authentication;
using Tozawa.Auth.Svc.Models.Converters;

namespace Tozawa.Auth.Svc.Controllers
{
    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, Models.Dtos.Backend.MemberDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateMemberCommandHandler> _logger;
        private readonly ILookupNormalizer _normalizer;

        public CreateMemberCommandHandler(ApplicationDbContext context, ILookupNormalizer normalizer, ILogger<CreateMemberCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.Dtos.Backend.MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var existingMember = await _context.Users
                           .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (existingMember != null && !existingMember.Deleted)
            {
                _logger.LogWarning("Member code already existing request {email}", request.Email);
                throw new ArgumentException("Member code already existing");
            }
            var org = await _context.Organizations.FirstOrDefaultAsync();
            var newuser = new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RootUser = false,
                OrganizationId = org.Id,
                Organization = org,
                UserCountry = request.Country
            };
            newuser.NormalizedEmail = _normalizer.NormalizeEmail(newuser.Email);
            newuser.EmailConfirmed = true;
            newuser.SecurityStamp = Guid.NewGuid().ToString();
            newuser.NormalizedUserName = _normalizer.NormalizeName(newuser.Email);
            _context.Users.Add(newuser);
            _context.SaveChanges();

            return MemberConverter.Convert(newuser);
        }
    }
}