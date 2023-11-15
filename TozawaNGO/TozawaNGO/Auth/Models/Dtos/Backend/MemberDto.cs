using System;

namespace TozawaNGO.Auth.Models.Dtos.Backend
{
    public class MemberDto : CurrentUserDto
    {
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public bool DeletedForever { get; set; }
    }
}


