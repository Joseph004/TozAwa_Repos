using System;

namespace Grains.Attachment.Models.Dtos;

public record CurrentUserRoleDto
{
    public Guid TextId { get; init; }
    public Guid OrganizationId { get; init; }
}
