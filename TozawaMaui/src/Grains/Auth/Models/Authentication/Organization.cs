using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.Auth.Models.Authentication;

public class Organization : CreatedModified
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Email { get; set; }
    [MaxLength(100)]
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public Guid DescriptionTextId { get; set; }
    public string Comment { get; set; }
    public Guid CommentTextId { get; set; }
    public bool Deleted { get; set; }
    public string City { get; set; }
    public string CityCode { get; set; }
    public string CountryCode { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public virtual ICollection<OrganizationFeature> Features { get; set; }
    public ICollection<ApplicationUser> OrganizationUsers { get; set; }
    public List<UserOrganization> UserOrganizations { get; set; }
    public virtual ICollection<Role> Roles { get; set; }
    public virtual ICollection<Station> Stations { get; set; }
    public ICollection<OrganizationAddress> Addresses { get; set; }
}
public class OrganizationAddress : CreatedModified
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Commun { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string CityCode { get; set; }
    public string CountryCode { get; set; }
    public string ZipCode { get; set; }
    public bool Active { get; set; }
    public Guid OrganizationId { get; set; }
    [ForeignKey("OrganizationId")]
    public Organization Organization { get; set; }
}

public class UserOrganization
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; }
    public bool PrimaryOrganization { get; set; }
}