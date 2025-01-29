using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters
{
    public interface ICurrentUserConverter
    {
        CurrentUserDto Convert(MemberItem user,
        List<CurrentUserOrganizationDto> organizations,
        List<AddressDto> addresses,
        List<RoleDto> roles,
        List<CurrentUserFunctionDto> functions);
    }

    public class CurrentUserConverter : ICurrentUserConverter
    {
        public CurrentUserConverter()
        {
        }

        public CurrentUserDto Convert(MemberItem user,
        List<CurrentUserOrganizationDto> organizations,
        List<AddressDto> addresses,
        List<RoleDto> roles,
        List<CurrentUserFunctionDto> functions)
        {
            return new()
            {
                Id = user.UserId,
                Admin = user.AdminMember,
                UserName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.UserCountry,
                Features = user.Features,
                Organizations = organizations,
                Addresses = addresses,
                Roles = roles,
                Gender = user.Gender,
                Functions = functions
            };
        }
    }
}