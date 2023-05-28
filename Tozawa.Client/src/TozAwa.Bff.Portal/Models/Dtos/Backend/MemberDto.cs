using System;

namespace Tozawa.Bff.Portal.Models.Dtos.Backend
{
    public class MemberDto : IdCodeEntity
    {
        public string Description { get; set; } = "";
        public Guid DescriptionTextId { get; set; }
        public string UserName { get; init; } = "";
        public string Email { get; init; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool RootUser { get; init; }
        public List<CurrentUserRoleDto> Roles { get; init; } = new();
        public List<CurrentUserFunctionDto> Functions { get; set; } = new();
        public Guid? FallbackLanguageId { get; init; }
        public Guid LanguageId { get; init; }
        public List<Guid> OrganizationLanguageIds { get; set; }
        public List<Guid> ExportLanguageIds { get; set; }
        public string Organization { get; init; } = "";
        public Guid Oid { get; init; }
        public List<int> Features { get; init; } = new();
        public bool IsFederatedUser { get; init; }
    }
}


