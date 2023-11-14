using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TozawaNGO.Auth.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public Guid UserId { get; set; }
        public Guid PartnerId { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string LastLoginCountry { get; set; } = "XXXXXXXXXXX";
        public string LastLoginCity { get; set; } = "XXXXXXXXXXX";
        public string LastLoginState { get; set; } = "XXXXXXXXXXX";
        public string LastLoginIPAdress { get; set; } = "xxxxxxxxx";
        public string Adress { get; set; }
        [NotMapped]
        public List<Role> Roles { get; set; } = new List<Role> { Role.None };
        public DateTime LastAttemptLogin { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string UserCountry { get; set; }
        public bool Deleted { get; set; }
        public bool AdminMember { get; set; }
        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }
        public DateTime? LastLogin { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; } = "";
        public DateTime? ModifiedDate { get; set; }
        public UserHashPwd UserHashPwd { get; set; }
    }
}