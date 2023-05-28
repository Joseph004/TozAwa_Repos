using System;

namespace Tozawa.Client.Portal.Models.Dtos;

public class CurrentUserFunctionDto
{
    public string FunctionType { get; set; } = "";
    public Guid TextId { get; set; }
}