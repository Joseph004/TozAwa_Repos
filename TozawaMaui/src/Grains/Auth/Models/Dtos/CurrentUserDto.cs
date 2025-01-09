
using Grains.Models;

namespace Grains.Auth.Models.Dtos;

public class CurrentUserDto
{
    public string UserName { get; init; } = "";
    public string Email { get; init; } = "";
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public bool Admin { get; init; }
    public List<int> Features { get; set; }
    public List<AddressDto> Addresses { get; set; }
    public List<CurrentUserOrganizationDto> Organizations { get; set; }
    public List<RoleDto> Roles { get; init; } = [];
    public string Country { get; set; } = "";
    public Guid Id { get; init; }
    public Guid WorkingOrganizationId { get; set; }
    public List<CurrentUserFunctionDto> Functions { get; set; } = [];
    public FunctionType[] GetFunctions()
    {
        return Roles != null
            ? Roles.SelectMany(x => x.Functions).Distinct().Select(x => x.FunctionType).Distinct().ToArray()
            : [];
    }
}