using System;

namespace Tozawa.Auth.Svc.Models.Dtos;

public record CurrentUserRoleDto
{
    public Guid TextId { get; init; }
    public Guid OrganizationId { get; init; }

    public ICollection<FunctionDto> Functions { get; set; } = new List<FunctionDto>();
}
