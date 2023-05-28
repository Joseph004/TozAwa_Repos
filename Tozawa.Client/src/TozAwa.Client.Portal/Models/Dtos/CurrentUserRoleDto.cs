using System;
using System.Collections.Generic;

namespace Tozawa.Client.Portal.Models.Dtos;

public record CurrentUserRoleDto
{
    public Guid TextId { get; init; }
    public Guid OrganizationId { get; init; }
}
