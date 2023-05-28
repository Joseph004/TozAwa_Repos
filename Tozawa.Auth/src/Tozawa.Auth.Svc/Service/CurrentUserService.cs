using Tozawa.Auth.Svc.extension;
using Tozawa.Auth.Svc.Models.Dtos;
using Tozawa.Auth.Svc.Models.Enums;

namespace Tozawa.Auth.Svc.Services;
public class CurrentUserService : ICurrentUserService
{
    public CurrentUserDto User { get; set; } = new();
    public CurrentUserService()
    {
    }
    public bool IsAuthorizedFor(params FunctionType[] functions)
    {
        try
        {
            if (User == null)
            {
                return false;
            }
            return User.RootUser || User.GetFunctions().ContainsAtLeastOne(functions);
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        return true;
    }

    public bool IsRoot()
    {
        if (User == null)
        {
            return false;
        }
        return User.RootUser;
    }

    public bool HasFeatures(IEnumerable<int> features)
    {
        return HasFeatures(features.ToArray());
    }

    public bool HasFeatures(params int[] features)
    {
        if (User?.Features == null)
        {
            return false;
        }

        return User.RootUser || features.AllMatching(User.Features);
    }

}
