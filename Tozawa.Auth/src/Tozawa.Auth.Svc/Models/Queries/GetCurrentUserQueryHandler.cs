using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tozawa.Auth.Svc.Context;
using Tozawa.Auth.Svc.Models.Authentication;
using Tozawa.Auth.Svc.Models.Converters;
using Tozawa.Auth.Svc.Models.Dtos;
using Tozawa.Auth.Svc.Services;

namespace Tozawa.Auth.Svc.Models.Queries
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserConverter _currentUserConverter;
        private readonly ICurrentCountry _currentCountry;
        private readonly ILookupNormalizer _normalizer;

        public GetCurrentUserQueryHandler(ApplicationDbContext context, ICurrentUserConverter currentUserConverter, ILookupNormalizer normalizer, ICurrentCountry currentCountry)
        {
            _context = context;
            _currentUserConverter = currentUserConverter;
            _normalizer = normalizer;
            _currentCountry = currentCountry;
        }

        public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            /* try
            {
                var newOrg = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = "Tozawa administartion",
                    Email = "tozawa2023@gmail.com",
                    PhoneNumber = "0701573032"
                };
                var org = _context.Organizations.First(x => x.Email == "tozawa2023@gmail.com");
                var newuser = new ApplicationUser
                {
                    UserId = Guid.NewGuid(),
                    UserName = "arobas.luhandu.root",
                    Email = "tozawa2023@gmail.com",
                    FirstName = "Joseph",
                    LastName = "Luhandu",
                    RootUser = true,
                    LockoutEnabled = true,
                    OrganizationId = org.Id,
                    Organization = org
                };
                newuser.NormalizedEmail = _normalizer.NormalizeEmail(newuser.Email);
                newuser.EmailConfirmed = true;
                newuser.SecurityStamp = Guid.NewGuid().ToString();
                newuser.NormalizedUserName = _normalizer.NormalizeName(newuser.UserName);
                //_context.Organizations.Add(newOrg);
                _context.Users.Add(newuser);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            } */

            var user = await _context.Users
                           .Include(x => x.Roles).ThenInclude(x => x.Role.Functions)
                           .Include(x => x.Organizations).ThenInclude(x => x.PartnerOrganizationsFrom)
                           .Include(x => x.Organizations).ThenInclude(x => x.PartnerOrganizationsTo)
                           .Include(x => x.Organizations).ThenInclude(x => x.Features)
                           .FirstOrDefaultAsync(x => x.UserId == request.Oid);

            if (user == null)
            {
                throw new ArgumentNullException();
            }

            var currentCountry = await _currentCountry.GetUserCountryByIp();

            if (!string.IsNullOrEmpty(currentCountry.Country))
            {
                user.LastLoginCountry = currentCountry.Country;
            }
            if (!string.IsNullOrEmpty(currentCountry.Ip))
            {
                user.LastLoginIPAdress = currentCountry.Ip;
            }
            if (!string.IsNullOrEmpty(currentCountry.State))
            {
                user.LastLoginState = currentCountry.State;
            }
            if (!string.IsNullOrEmpty(currentCountry.City))
            {
                user.LastLoginCity = currentCountry.City;
            }
            user.LastLogin = DateTime.UtcNow;
            _context.SaveChanges();

            var response = _currentUserConverter.Convert(user);

            return await Task.FromResult(response);

        }
    }
}