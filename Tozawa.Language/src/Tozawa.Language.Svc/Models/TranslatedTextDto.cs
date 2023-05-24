using System;
using Tozawa.Language.Svc.Context;

namespace Tozawa.Language.Svc.Models
{
    public class TranslatedTextDto
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }        
        public XliffState XliffState { get; set; }
        public bool Deleted { get; set; }        
        public Guid SystemTypeId { get; set; }        
    }
}