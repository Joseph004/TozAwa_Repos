using System;

namespace TozAwaHome.Models.Dtos;

public class CurrentUserFunctionDto
{
    public string FunctionType { get; set; } = "";
    public Guid TextId { get; set; }
}