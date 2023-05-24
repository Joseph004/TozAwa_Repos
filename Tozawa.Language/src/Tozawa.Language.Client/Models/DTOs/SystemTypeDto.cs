using System;

namespace Tozawa.Language.Client.Models.DTOs
{
    public class SystemTypeDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsDefault { get; set; }
    }
}