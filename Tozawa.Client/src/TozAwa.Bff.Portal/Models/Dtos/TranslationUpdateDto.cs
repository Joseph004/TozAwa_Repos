
using System;

namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class TranslationUpdateDto
    {
        public string Text { get; set; }
        public Guid Id { get; set; }
        public Guid LanguageId { get; set; }
        public Guid SystemTypeId { get; set; }
    }

}
