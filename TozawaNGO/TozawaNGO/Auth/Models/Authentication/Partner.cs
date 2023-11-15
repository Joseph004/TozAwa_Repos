using System.ComponentModel.DataAnnotations;

namespace TozawaNGO.Auth.Models.Authentication
{
    public class Partner : CreatedModified
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
        public string Adress { get; set; }
        public bool Deleted { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}