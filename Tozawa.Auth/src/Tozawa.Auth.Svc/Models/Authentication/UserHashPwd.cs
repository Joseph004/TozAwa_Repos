using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tozawa.Auth.Svc.Models.Authentication
{
    public class UserHashPwd : CreatedModified
    {
        [Key]
        public Guid Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }
    }
}