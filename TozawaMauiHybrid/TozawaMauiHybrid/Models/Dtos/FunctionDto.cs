using TozawaMauiHybrid.Models.Enums;

namespace TozawaMauiHybrid.Models.Dtos;

public class FunctionDto
{
    public FunctionType FunctionType { get; set; }
    public Guid RoleId { get; set; }
}