using System;
using System.Collections.Generic;

namespace TozawaNGO.Models.Dtos;

public class CurrentUserDto : BaseDto
{
    public string UserName { get; init; } = "";
    public string Email { get; set; } = "";
    public bool Admin { get; init; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public List<RoleDto> Roles { get; init; } = new();
    public Guid PartnerId { get; init; }
    public string Partner { get; init; } = "";
}