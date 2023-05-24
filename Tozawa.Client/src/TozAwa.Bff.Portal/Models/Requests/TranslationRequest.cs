
using System;
using System.Collections.Generic;

namespace Tozawa.Bff.Portal.Models
{
    public class TranslationRequest
    {
        public Guid LanguageId { get; set; }
        public string Text { get; set; }
    }
}