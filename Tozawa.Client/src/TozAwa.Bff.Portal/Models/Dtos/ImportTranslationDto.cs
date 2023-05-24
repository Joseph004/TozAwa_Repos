
using System;

namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class ImportTranslationDto
    {
        public Guid SystemTypeId { get; set; }
        public ImportTranslationTextDto Original { get; set; }
        public List<ImportTranslationTextDto> Translations { get; set; }
    }
}
