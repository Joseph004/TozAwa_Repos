using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tozawa.Auth.Svc.Models.Authentication 
{
    [Table("UserRoles")]
    public class UserRole : CreatedModified
    {
        [Column("User_Id")]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = new();
        [Column("Role_Id")]
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = new();

        public Guid OrganizationId { get; set; }
        public bool Active { get; set; }
    }
}
