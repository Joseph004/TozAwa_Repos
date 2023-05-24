using System.ComponentModel.DataAnnotations.Schema;
using Tozawa.Auth.Svc.Models.Enums;

namespace Tozawa.Auth.Svc.Models.Authentication
{
    public class Function : CreatedModified
    {
        [Column(Order = 1)]
        public FunctionType Functiontype { get; set; }

        [Column(Order = 2)]
        public virtual Guid RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = new();
    }
}
