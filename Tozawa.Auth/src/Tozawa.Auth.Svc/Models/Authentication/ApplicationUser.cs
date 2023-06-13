using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Tozawa.Auth.Svc.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid DescriptionTextId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string LastLoginCountry { get; set; } = "XXXXXXXXXXX";
        public string LastLoginCity { get; set; } = "XXXXXXXXXXX";
        public string LastLoginState { get; set; } = "XXXXXXXXXXX";
        public string LastLoginIPAdress { get; set; } = "xxxxxxxxx";
        public DateTime LastAttemptLogin { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string UserCountry { get; set; }
        public bool Deleted { get; set; }
        public Guid WorkingOrganizationId { get; set; }
        public bool RootUser { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; }
        public Guid Oid { get; set; }
        public DateTime? LastLogin { get; set; }
        public ICollection<Organization> Organizations { get; set; }
        public List<UserOrganization> UserOrganizations { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; } = "";
        public DateTime? ModifiedDate { get; set; }
        public UserHashPwd UserHashPwd { get; set; }
    }
    public class UserOrganization
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = new();
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = new();
    }
}