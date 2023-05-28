using System;
using System.Collections.Generic;
using Tozawa.Bff.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Models.Dtos;

public class CurrentUserDto : BaseDto
{
    public string UserName { get; init; } = "";
    public string Email { get; init; } = "";
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    // public string UserAvatar { get; set; }
    public string FolderId { get; init; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public bool RootUser { get; init; }
    public List<CurrentUserRoleDto> Roles { get; init; } = new();
    public List<CurrentUserFunctionDto> Functions { get; set; } = new();
    public Guid OrganizationId { get; init; }
    public Guid? FallbackLanguageId { get; init; }
    public Guid LanguageId { get; init; }
    public List<Guid> OrganizationLanguageIds { get; set; }
    public List<Guid> ExportLanguageIds { get; set; }
    public string Organization { get; init; } = "";
    public Guid Oid { get; init; }
    public List<int> Features { get; init; } = new();
    public bool IsFederatedUser { get; init; }

    public FunctionType[] GetFunctions()
    {
        return Roles != null
            ? Roles.SelectMany(x => x.Functions).Distinct().Select(x => x.FunctionType).Distinct().ToArray()
            : new FunctionType[0];
    }

    public List<CurrentUserOrganizationDto> Organizations { get; init; } = new();
}