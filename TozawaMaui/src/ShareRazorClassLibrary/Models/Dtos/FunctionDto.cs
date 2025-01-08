using ShareRazorClassLibrary.Models.Enums;

namespace ShareRazorClassLibrary.Models.Dtos;

public class FunctionDto
{
    public FunctionType FunctionType { get; set; }
    public Guid RoleId { get; set; }
}