namespace ShareRazorClassLibrary.Models.Dtos;

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid DescriptionTextId { get; set; }
    public string Comment { get; set; }
    public Guid CommentTextId { get; set; }
    public List<AddressDto> Addresses { get; set; }
    public string Code { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreateDate { get; set; }
    public int AttachmentsCount { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<int> Features { get; set; }
    public bool Deleted { get; set; }
    public List<RoleDto> Roles { get; set; }
}