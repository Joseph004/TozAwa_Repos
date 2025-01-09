using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters
{
    public interface ICurrentUserConverter
    {
        CurrentUserDto Convert(ApplicationUser user);
    }

    public class CurrentUserConverter : ICurrentUserConverter
    {
        public CurrentUserConverter()
        {
        }

        public CurrentUserDto Convert(ApplicationUser user)
        {
            return new()
            {
                Id = user.UserId,
                Admin = user.AdminMember,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.UserCountry,
                Features = user.Organizations?.SelectMany(o => o.Features) != null
            ? user.Organizations?.SelectMany(o => o.Features).Select(x => x.Feature).ToList()
            : [],
                Organizations = user.Organizations
                .Select(org => new Models.Dtos.CurrentUserOrganizationDto
                {
                    Id = org.Id,
                    Name = org.Name,
                    Features = org.Features != null
                        ? org.Features.Select(x => x.Feature).ToList()
                        : [],
                    Active = true,
                    PrimaryOrganization = user.UserOrganizations.First(u => u.OrganizationId == org.Id).PrimaryOrganization
                })
                .ToList(),
                Addresses = user.Addresses.Select(x => new AddressDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    City = x.City,
                    State = x.State,
                    Country = x.Country,
                    ZipCode = x.ZipCode,
                    Active = x.Active
                }).ToList(),
                Roles = user.Roles
                .Select(userRole => new RoleDto
                {
                    Id = userRole.RoleId,
                    OrganizationId = userRole.OrganizationId,
                    Role = (Grains.Auth.Models.Dtos.Role)userRole.Role.RoleEnum
                })
                .ToList(),
                Functions = user.Roles
                .SelectMany(x => x.Role.Functions)
                .Select(function => function.Functiontype)
                .Distinct()
                .Select(functionType => new CurrentUserFunctionDto
                {
                    FunctionType = functionType
                })
                .ToList()
            };
        }
    }
}