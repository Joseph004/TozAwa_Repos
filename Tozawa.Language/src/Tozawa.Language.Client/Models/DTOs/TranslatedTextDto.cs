using Tozawa.Language.Client.Models.Enum;
using System;

namespace Tozawa.Language.Client.Models.DTOs
{
    public class TranslatedTextDto
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public XliffState XliffState { get; set; }
        public bool Deleted { get; set; }
        public Guid SystemTypeId { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
