
using System;
using System.Collections.Generic;

namespace TozawaMauiHybrid.Models.FormModels
{
    public class TranslationRequest
    {
        public Guid LanguageId { get; set; }
        public string Text { get; set; }
    }
}