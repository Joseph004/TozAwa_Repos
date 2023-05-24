
using System;
using System.Collections.Generic;

namespace Tozawa.Client.Portal.Models.FormModels
{
    public class TranslationRequest
    {
        public Guid LanguageId { get; set; }
        public string Text { get; set; }
    }
}