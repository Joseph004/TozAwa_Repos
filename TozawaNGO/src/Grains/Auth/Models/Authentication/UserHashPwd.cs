using System.ComponentModel.DataAnnotations;

namespace Grains.Auth.Models.Authentication
{
    public class UserHashPwd : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Guid UserId { get; set; }
        public string PasswordSalt { get; set; }
    }
}