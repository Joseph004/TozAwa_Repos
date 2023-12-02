using MediatR;
using Microsoft.AspNetCore.Identity;
using TozawaNGO.Context;
using TozawaNGO.Auth.Models.Converters;
using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Dtos.Backend;
using TozawaNGO.Services;

namespace TozawaNGO.Auth.Models.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, MemberDto>
    {
        private readonly TozawangoDbContext _context;
        private readonly ILookupNormalizer _normalizer;
        private readonly IPasswordHashService _passwordHashService;

        public CreateUserCommandHandler(TozawangoDbContext context, ILookupNormalizer normalizer, IPasswordHashService passwordHashService)
        {
            _context = context;
            _normalizer = normalizer;
            _passwordHashService = passwordHashService;
        }

        public async Task<MemberDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var partner = _context.Partners.First(x => x.Email == "tozawango@gmail.com");
            var newuser = new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = request.Roles.Select(x => (Role)x).ToList(),
                AdminMember = false,
                LockoutEnabled = true,
                PartnerId = partner.Id,
                Partner = partner
            };
            newuser.NormalizedEmail = _normalizer.NormalizeEmail(newuser.Email);
            newuser.EmailConfirmed = true;
            newuser.SecurityStamp = Guid.NewGuid().ToString();
            newuser.NormalizedUserName = _normalizer.NormalizeName(newuser.UserName);
            _context.TzUsers.Add(newuser);
            if (!string.IsNullOrEmpty(request.Password))
            {
                var hash = _passwordHashService.HashPasword(request.Password, out var salt);
                newuser.UserPasswordHash = hash;
                var userSalt = Convert.ToHexString(salt);
                var hashSalt = new UserHashPwd
                {
                    Id = Guid.NewGuid(),
                    ApplicationUser = newuser,
                    UserId = newuser.UserId,
                    PasswordSalt = userSalt
                };
                _context.UserHashPwds.Add(hashSalt);
            }

            _context.SaveChanges();

            var response = MemberConverter.Convert(newuser);

            return await Task.FromResult(response);

        }
    }
}