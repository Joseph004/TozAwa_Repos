using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.Auth.Models.Authentication;

public class OrganizationFeature : CreatedModified
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
    public int Feature { get; set; }
}