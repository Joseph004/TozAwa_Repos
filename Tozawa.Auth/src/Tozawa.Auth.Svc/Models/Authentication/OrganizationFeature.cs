using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tozawa.Auth.Svc.Models.Authentication
{
    public class OrganizationFeature : CreatedModified
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; } = new();

        public int Feature { get; set; }
    }
}
