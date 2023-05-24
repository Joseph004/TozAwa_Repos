

using System;

namespace TozAwaHome.Models.Dtos
{
    public class TranslationDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsTranslated { get; set; }
    }
}
