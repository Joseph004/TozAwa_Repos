using System;

namespace Tozawa.Language.Client.Models.FormModels
{
    public class AddTextCommand
    {
        public string Text { get; set; }
        public Guid LanguageId { get; set; }        
        public Guid SystemTypeId { get; set; }
    }
}
