using System;
using Tozawa.Language.Svc.Context;

namespace Tozawa.Language.Svc.Models
{
    public class TranslationDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool Deleted { get; set; }
        public Guid LanguageId { get; set; }
        public XliffState XliffState { get; set; }
        public Guid SystemTypeId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOriginalText { get; set; }
    }
}   