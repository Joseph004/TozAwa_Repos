using System;
using Tozawa.Language.Client.Models.Enum;

namespace Tozawa.Language.Client.Models.DTOs
{
    public class XliffDistributionFile
    {
        public int id { get; set; }
        public string fileId { get; set; }
        public string fileName { get; set; }
        public XliffFileState fileState { get; set; }
        public string fileStateDescription { get; set; }
        public string sourceLanguageLongName { get; set; }
        public string targetLanguageLongName { get; set; }
        public int numberOfTranslations { get; set; }
        public int numberOfWordsSentInSourceLanguage { get; set; }
        public DateTime requestedDeliveryDate { get; set; }
        public string createdBy { get; set; }
        public DateTime createdAt { get; set; }        
    }
}