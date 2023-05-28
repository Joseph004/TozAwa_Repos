using System;
using System.Collections.Generic;
using System.Linq;
using TozAwaHome.Models.Enums;

namespace TozAwaHome.Models.Dtos
{
    public class UserDto
    {
        public string Password { get; set; }
        public List<Guid> LanguageIds { get; set; }
        public Dictionary<string, string> Custom { get; set; }
        public List<int> Features { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastLogin { get; set; }
        public Guid Oid { get; set; }
        public List<RoleDto> Roles { get; set; }
        public bool SuperAdmin { get; set; }
        public Guid? FallbackLanguageId { get; set; }
        public Guid OrganizationId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public Guid Id { get; set; }
        public FunctionType[] GetFunctions;
        public bool IsAuthenticated { get; set; }
    }
}


