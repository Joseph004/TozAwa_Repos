using System;

namespace Tozawa.Language.Svc.Models
{
    public class LanguageDto
    {
        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public bool Deleted { get; set; }
        public bool IsDefault { get; set; }
    }
}