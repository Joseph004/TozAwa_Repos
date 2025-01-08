using Grains.Extension;
using Grains.Auth.Models.Dtos;
using Grains.Models;

namespace Grains.Auth.Services;
public class CurrentUserService : ICurrentUserService
{
    public CurrentUserDto User { get; set; } = new();
    public Guid LanguageId { get; set; }
    public CurrentUserService()
    {
    }
    public bool IsAuthorizedFor(params FunctionType[] functions)
    {
        var response = true;
        try
        {
            if (User == null)
            {
                response = false;
            }
            return User.Admin || User.GetFunctions().ContainsAtLeastOne(functions);
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
            response = false;
        }
        return response;
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
