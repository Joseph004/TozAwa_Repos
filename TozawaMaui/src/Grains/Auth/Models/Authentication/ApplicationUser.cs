using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grains.Auth.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public Guid DescriptionTextId { get; set; }
        public string Comment { get; set; }
        public Guid CommentTextId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string LastLoginCountry { get; set; } = "XXXXXXXXXXX";
        public string LastLoginCity { get; set; } = "XXXXXXXXXXX";
        public string LastLoginState { get; set; } = "XXXXXXXXXXX";
        public string LastLoginIPAdress { get; set; } = "xxxxxxxxx";
        public virtual ICollection<UserRole> Roles { get; set; }
        public ICollection<Organization> Organizations { get; set; }
        public List<UserOrganization> UserOrganizations { get; set; }
        public DateTime LastAttemptLogin { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string UserCountry { get; set; }
        public string UserCity { get; set; }
        public string CityCode { get; set; }
        public string CountryCode { get; set; }
        public bool Deleted { get; set; }
        public bool AdminMember { get; set; }
        public DateTime? LastLogin { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; } = "";
        public Gender Gender { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<Guid> StationIds { get; set; }
        public ICollection<UserAddress> Addresses { get; set; }
        public ICollection<UserLandLord> LandLords { get; set; }
        public ICollection<UserTenant> Tenants { get; set; }
    }

    public class UserLandLord : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public ApplicationUser UserLandLord_LandLordUser { get; set; }
        public Guid UserLandLord_TenantUserId { get; set; }
        [ForeignKey(nameof(UserLandLord_TenantUserId))]
        public ApplicationUser UserLandLord_TenantUser { get; set; }
    }

    public class UserTenant : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserTenant_LandLordUserId { get; set; }
        [ForeignKey(nameof(UserTenant_LandLordUserId))]
        public ApplicationUser UserTenant_LandLordUser { get; set; }
        public ApplicationUser UserTenant_TenantUser { get; set; }
    }

    public class UserAddress : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string CountryCode { get; set; }
        public string Commun { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool Active { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

    public enum Gender
    {
        None = 0,
        Male = 1,
        Female = 2,
    }



    internal class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(e => e.StationIds).HasConversion<ListOfGuidsCoverter, ListOfGuidsComparer>();
        }
    }
}