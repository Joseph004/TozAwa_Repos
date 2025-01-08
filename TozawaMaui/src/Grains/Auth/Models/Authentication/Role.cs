
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.Auth.Models.Authentication
{
    public class UserRole : CreatedModified
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Guid OrganizationId { get; set; }
    }
    public class Role
    {
        [Key]
        public Guid Id { get; set; }
        public RoleEnum RoleEnum { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public ICollection<UserRole> Users { get; set; }
        public ICollection<Function> Functions { get; set; }

    }

    public enum RoleEnum
    {
        None = 0,
        President = 1,
        VicePresident = 2,
        Cashier = 3,
        Signatory = 4,
        LandLoard = 5,
        Tenant = 6,
        economist = 7,
        CareTaker = 8 //vakmästare
    }
}
