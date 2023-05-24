namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class TranslationAddDto
    {
        public string Text { get; set; }
        public Guid LanguageId { get; set; }
        public Guid SystemTypeId { get; set; }
    }

}