
namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class FileDto
    {
        public string Name { get; set; } = "";
        public string FileName { get; set; } = "";
        public string Content { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long ContentLength { get; set; }
        public string Extension => Path.GetExtension(Name);
    }
}