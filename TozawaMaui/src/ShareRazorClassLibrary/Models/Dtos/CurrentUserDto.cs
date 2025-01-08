
using ShareRazorClassLibrary.Models.Enums;

namespace ShareRazorClassLibrary.Models.Dtos;

public class CurrentUserDto : BaseDto
{
    public string UserName { get; init; } = "";
    public string Email { get; set; } = "";
    public bool Admin { get; init; }
    public Guid WorkingOrganizationId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public List<RoleDto> Roles { get; init; } = [];
    public List<int> Features { get; set; }
    public List<AddressDto> Addresses { get; set; }
    public List<CurrentUserOrganizationDto> Organizations { get; set; }
    public Guid PartnerId { get; init; }
    public string Partner { get; init; } = "";
    public string Country { get; set; } = "";
    public List<CurrentUserFunctionDto> Functions { get; set; } = [];
    public FunctionType[] GetFunctions()
    {
        return Roles != null
            ? Roles.SelectMany(x => x.Functions).Distinct().Select(x => x.FunctionType).Distinct().ToArray()
            : [];
    }
}