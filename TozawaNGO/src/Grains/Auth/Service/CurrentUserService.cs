using Grains.Extension;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Services;
public class CurrentUserService : ICurrentUserService
{
    public CurrentUserDto User { get; set; } = new();
    public Guid LanguageId { get; set; }
    public CurrentUserService()
    {
    }
    public bool IsAuthorizedFor(params RoleDto[] roles)
    {
        try
        {
            if (User == null)
            {
                return false;
            }
            return User.Admin || User.Roles.ContainsAtLeastOne(roles);
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        return true;
    }

    public bool IsAdmin()
    {
        if (User == null)
        {
            return false;
        }
        return User.Admin;
    }
}
