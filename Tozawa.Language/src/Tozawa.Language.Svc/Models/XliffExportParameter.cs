using System;

namespace Tozawa.Language.Svc.Models
{
    public class XliffExportParameter
    {
        public Guid SourceLanguageId { get; set; }
        public Guid TargetLangaugeId { get; set; }        
        public Guid systemTypeId { get; set; }
    }
}
