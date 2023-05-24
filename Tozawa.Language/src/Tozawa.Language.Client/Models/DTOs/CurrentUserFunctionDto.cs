using System;

namespace Tozawa.Language.Client.Models.Dtos;

public class CurrentUserFunctionDto
{
    public string FunctionType { get; set; } = "";
    public Guid TextId { get; set; }
}