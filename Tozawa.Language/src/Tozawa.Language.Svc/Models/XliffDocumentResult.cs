using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tozawa.Language.Svc.Context;

namespace Tozawa.Language.Svc.Models
{
    public class XliffDocumentResult
    {
        public XDocument XDocument { get; set; }
        public List<Translation> Existing { get; set; }
        public List<Translation> NonExisting { get; set; }
        public Guid FileIdForLog { get; set; }
        public Guid SourceLanguageForLog { get; set; }
        public Guid TargetLanguageForLog { get; set; }
        public List<TranslateableText> TranslatableTexts { get; set; }        
    }
}