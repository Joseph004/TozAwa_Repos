namespace Grains.Auth.Models.Dtos;

public class CurrentUserOrganizationDto
{
    public Guid Id { get; set; }
    public List<AddressDto> Addresses { get; set; }
    public List<int> Features { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public bool PrimaryOrganization { get; set; }
}