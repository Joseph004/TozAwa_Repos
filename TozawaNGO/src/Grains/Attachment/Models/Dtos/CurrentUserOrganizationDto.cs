using System;

namespace Grains.Attachment.Models.Dtos;

public record CurrentUserOrganizationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public bool Active { get; init; }
}