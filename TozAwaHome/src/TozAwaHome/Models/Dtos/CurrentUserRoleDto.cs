using System;
using System.Collections.Generic;

namespace TozAwaHome.Models.Dtos;

public record CurrentUserRoleDto
{
    public Guid TextId { get; init; }
    public Guid OrganizationId { get; init; }
}
