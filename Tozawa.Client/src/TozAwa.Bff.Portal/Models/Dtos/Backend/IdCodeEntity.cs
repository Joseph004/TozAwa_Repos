using System;

namespace Tozawa.Bff.Portal.Models.Dtos.Backend
{
    public class IdCodeEntity
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public bool DeleteForever { get; set; }
        public Guid OrganizationId { get; set; }
        public virtual string Code { get; set; } = "";
        public virtual string CreatedBy { get; set; } = "";
        public virtual DateTime CreatedDate { get; set; }
    }
}


