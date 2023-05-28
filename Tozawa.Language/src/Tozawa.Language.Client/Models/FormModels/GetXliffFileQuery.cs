using System;

namespace Tozawa.Language.Client.Models.FormModels
{
    public class GetXliffFileQuery
    {
        public Guid OriginalLanguageId { get; set; }
        public Guid TargetLanguageId { get; set; }        
        public Guid SystemTypeId { get; set; }
    }
}