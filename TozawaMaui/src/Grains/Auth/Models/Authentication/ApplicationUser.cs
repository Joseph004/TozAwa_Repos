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
        public List<Guid> Tenants { get; set; } = [];
        public List<Guid> LandLords { get; set; } = [];
        public DateTime LastAttemptLogin { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string UserCountry { get; set; }
        public string UserCity { get; set; }
        public bool Deleted { get; set; }
        public bool AdminMember { get; set; }
        public DateTime? LastLogin { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; } = "";
        public DateTime? ModifiedDate { get; set; }
        public List<Guid> StationIds { get; set; }
        public ICollection<UserAddress> Addresses { get; set; }
    }

    public class UserAddress : CreatedModified
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool Active { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

    internal class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(e => e.StationIds).HasConversion<ListOfGuidsCoverter, ListOfGuidsComparer>();
            builder.Property(e => e.Tenants).HasConversion<ListOfGuidsCoverter, ListOfGuidsComparer>();
            builder.Property(e => e.LandLords).HasConversion<ListOfGuidsCoverter, ListOfGuidsComparer>();
        }
    }
}