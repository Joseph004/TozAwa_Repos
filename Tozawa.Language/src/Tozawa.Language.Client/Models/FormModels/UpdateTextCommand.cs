using Tozawa.Language.Client.Models.Enum;
using System;

namespace Tozawa.Language.Client.Models.FormModels
{
    public class UpdateTextCommand
    {
        public string Text { get; set; }
        public Guid Id { get; set; }
        public Guid LanguageId { get; set; }
        public Guid SystemTypeId { get; set; }
        public XliffState XliffState { get; set; }
    }
}
