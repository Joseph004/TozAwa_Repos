using System;

namespace Tozawa.Language.Svc.Models.Dtos;

public record CurrentUserOrganizationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public bool Active { get; init; }
}