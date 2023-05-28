using System.Collections.Generic;

namespace Tozawa.Language.Svc.Models
{
    public class XliffFile
    {
        public XliffFileInfo XliffFileInfo { get; set; }
        public List<TranslateableText> TranslateableTexts { get; set; }
    }
}