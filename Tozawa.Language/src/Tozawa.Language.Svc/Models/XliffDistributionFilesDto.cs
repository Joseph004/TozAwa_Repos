using System;
using Tozawa.Language.Svc.Context;

namespace Tozawa.Language.Svc.Models
{
    public class XliffDistributionFilesDto
    {
        public int Id { get; set; }
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public XliffFileState FileState { get; set; }
        public string FileStateDescription { get; set; }
        public string SourceLanguageLongName { get; set; }
        public string TargetLanguageLongName { get; set; }
        public int NumberOfTranslations { get; set; }
        public int? NumberOfWordsSentInSourceLanguage { get; set; }
        public DateTime? RequestedDeliveryDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}