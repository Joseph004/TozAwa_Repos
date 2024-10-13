namespace TozawaMauiHybrid.Models.Dtos
{
    public class ActiveLanguageDto
    {
        public Guid Id => Guid.Parse(StringId);
        public string StringId { get; set; }
        public string ShortName { get; set; }
        public string Culture { get; set; }
        public string LongName { get; set; }
    }
}
