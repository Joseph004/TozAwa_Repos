using Grains.Auth.Models.Authentication;
using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters;

public static class RoleConverter
{
    public static RoleDto Convert(Grains.Auth.Models.Authentication.Role role) => new()
    {
        Id = role.Id,
        Functions = role.Functions != null ? role.Functions.Select(Convert).ToList() : [],
        OrganizationId = role.OrganizationId
    };
    public static Grains.Auth.Models.Authentication.Role Convert(RoleDto role) => new()
    {
        Id = role.Id,
        Functions = role.Functions != null ? role.Functions.Select(Convert).ToList() : [],
        OrganizationId = role.OrganizationId
    };

    private static FunctionDto Convert(Function func) => new()
    {
        FunctionType = func.Functiontype,
        RoleId = func.RoleId
    };
    private static Function Convert(FunctionDto func) => new()
    {
        Functiontype = func.FunctionType,
    };

}