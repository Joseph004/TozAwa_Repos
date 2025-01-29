using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Models.Converters;

public static class RoleConverter
{
    public static RoleDto Convert(RoleItem role, List<FunctionDto> functions) => new()
    {
        Id = role.Id,
        Functions = functions,
        OrganizationId = role.OrganizationId
    };
}