using TozawaNGO.Auth.Models.Authentication;
using TozawaNGO.Auth.Models.Dtos;

namespace TozawaNGO.Auth.Models.Converters
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
                Adress = user.Adress,
                Roles = user.Roles.Select(x => (RoleDto)x).ToList(),
                PartnerId = user.PartnerId,
                Partner = user.Partner.Name
            };

        }
    }
}