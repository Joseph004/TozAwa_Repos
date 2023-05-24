using Tozawa.Language.Client.Models.Enum;
using System;

namespace Tozawa.Language.Client.Models.FormModels
{
    public class GetTranslationSummaryQuery
    {       
        public Guid SourceLanguageId { get; set; }
        public Guid TargetLanguageId { get; set; }        
        public Guid? SystemTypeId { get; set; }
        public string FilterText { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 35;
        public XliffState? XliffState { get; set; }
    }
}
