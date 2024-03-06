
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Grains.Context;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Converters;
using Microsoft.Extensions.Logging;

namespace Grains.Auth.Controllers
{
    public class CreateMemberCommandHandler(TozawangoDbContext context, ILookupNormalizer normalizer, ILogger<CreateMemberCommandHandler> logger) : IRequestHandler<CreateMemberCommand, Models.Dtos.Backend.MemberDto>
    {
        private readonly TozawangoDbContext _context = context;
        private readonly ILogger<CreateMemberCommandHandler> _logger = logger;
        private readonly ILookupNormalizer _normalizer = normalizer;

        public async Task<Models.Dtos.Backend.MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var existingMember = await _context.TzUsers
                           .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);

            if (existingMember != null && !existingMember.Deleted)
            {
                _logger.LogWarning("Member email already existing request {email}", request.Email);
                throw new ArgumentException("Member email already existing");
            }
            var partner = _context.Partners.First(x => x.Email == "tozawango@gmail.com");
            var newuser = new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AdminMember = false,
                UserCountry = request.Country,
                PartnerId = partner.Id,
                Partner = partner
            };
            newuser.NormalizedEmail = _normalizer.NormalizeEmail(newuser.Email);
            newuser.EmailConfirmed = true;
            newuser.SecurityStamp = Guid.NewGuid().ToString();
            newuser.NormalizedUserName = _normalizer.NormalizeName(newuser.Email);
            _context.TzUsers.Add(newuser);
            _context.SaveChanges();

            return MemberConverter.Convert(newuser);
        }
    }
}