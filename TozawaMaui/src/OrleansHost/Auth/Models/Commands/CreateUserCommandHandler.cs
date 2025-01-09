using MediatR;
using Microsoft.AspNetCore.Identity;
using Grains.Context;
using Grains.Auth.Models.Converters;
using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos.Backend;
using Grains.Services;
using Grains.Auth.Models.Dtos;

namespace OrleansHost.Auth.Models.Commands
{
    public class CreateUserCommandHandler(TozawangoDbContext context, ILookupNormalizer normalizer, IPasswordHashService passwordHashService) : IRequestHandler<CreateUserCommand, MemberDto>
    {
        private readonly TozawangoDbContext _context = context;
        private readonly ILookupNormalizer _normalizer = normalizer;
        private readonly IPasswordHashService _passwordHashService = passwordHashService;

        public async Task<MemberDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var newuser = new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AdminMember = false,
                LockoutEnabled = true
            };
            newuser.NormalizedEmail = _normalizer.NormalizeEmail(newuser.Email);
            newuser.EmailConfirmed = true;
            newuser.SecurityStamp = Guid.NewGuid().ToString();
            newuser.NormalizedUserName = _normalizer.NormalizeName(newuser.UserName);
            _context.TzUsers.Add(newuser);

            _context.SaveChanges();
            var addressDto = newuser.Addresses.Select(x => new AddressDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                City = x.City,
                State = x.State,
                Country = x.Country,
                ZipCode = x.ZipCode,
                Active = x.Active
            }).ToList();
            var response = MemberConverter.Convert(newuser, addressDto);

            return await Task.FromResult(response);

        }
    }
}