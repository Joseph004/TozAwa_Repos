using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tozawa.Auth.Svc.Models.Authentication
{
    public class Organization : CreatedModified
    {
        public Organization()
        {
            LanguageIds = new List<Guid>();
            ExportLanguageIds = new List<Guid>();
        }
        [Key]
        public Guid Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(100)]
        public string PhoneNumber { get; set; }
        public Guid? FallbackLanguageId { get; set; }
        public List<Guid> LanguageIds { get; set; }
        public List<Guid> ExportLanguageIds { get; set; }
        public virtual ICollection<OrganizationFeature> Features { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public bool IsFederated { get; set; }
        public ICollection<ApplicationUser> OrganizationUsers { get; set; }
        public List<UserOrganization> UserOrganizations { get; set; }

        public List<PartnerOrganization> PartnerOrganizationsTo { get; set; }
        public List<PartnerOrganization> PartnerOrganizationsFrom { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }

    public class PartnerOrganization
    {
        public Guid FromId { get; set; }
        public Guid ToId { get; set; }
        public Organization OrganizationFrom { get; set; } = new();
        public Organization OrganizationTo { get; set; } = new();

    }
}