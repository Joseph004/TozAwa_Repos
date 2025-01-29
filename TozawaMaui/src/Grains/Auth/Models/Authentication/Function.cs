using System.ComponentModel.DataAnnotations.Schema;
using Grains.Models;

namespace Grains.Auth.Models.Authentication;

public class Function : CreatedModified
{
    [Column(Order = 1)]
    public FunctionType FunctionType { get; set; }

    [Column(Order = 2)]
    public virtual Guid RoleId { get; set; }

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; }
}