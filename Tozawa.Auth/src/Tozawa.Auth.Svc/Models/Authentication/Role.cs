using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Tozawa.Auth.Svc.Models.Authentication
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = new();
        public Guid TextId { get; set; }

        public ICollection<UserRole> Users { get; set; } = new Collection<UserRole>();

        public ICollection<Function> Functions { get; set; } = new Collection<Function>();

    }
}
