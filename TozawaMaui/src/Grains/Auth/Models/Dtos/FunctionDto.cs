using Grains.Models;

namespace Grains.Auth.Models.Dtos;

public class FunctionDto
{
    public FunctionType FunctionType { get; set; }
    public Guid RoleId { get; set; }
}