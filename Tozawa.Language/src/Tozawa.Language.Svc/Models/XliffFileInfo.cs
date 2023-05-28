using System;

namespace Tozawa.Language.Svc.Models
{
    public class XliffFileInfo
    {
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public string FileId { get; set; }
        public string ReqDeliverDate { get; set; }
        public string Datatype { get; set; }
        public string Original { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}