using System;

namespace Tozawa.Auth.Svc.Models.Dtos.Backend
{
    public class MemberDto : CurrentUserDto
    {
        public Guid DescriptionTextId { get; set; }
        public bool Deleted { get; set; }
        public bool DeletedForever { get; set; }
    }
}


