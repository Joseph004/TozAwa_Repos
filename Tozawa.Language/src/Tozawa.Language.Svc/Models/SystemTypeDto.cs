using System;

namespace Tozawa.Language.Svc.Models
{
    public class SystemTypeDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsDefault { get; set; }
    }
}