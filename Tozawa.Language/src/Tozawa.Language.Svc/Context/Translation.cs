using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Tozawa.Language.Svc.Context
{
    public class Translation : BaseEntity
    {
        public Guid TextId { get; set; }
        public Guid Id { get; set; }
        public Guid LanguageId { get; set; }
        public Guid SystemTypeId { get; set; }
        public XliffState XliffState { get; set; }
        public string Text { get; set; }
        public bool Deleted { get; set; }
        public bool IsOriginalText { get; set; }
        public virtual Languagemd Language { get; set; }
        public virtual SystemType SystemType { get; set; }
    }
}
