
namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class AttachmentDownloadDto
    {
        public string Name { get; init; } = "";
        public byte[] Content { get; init; } = Array.Empty<byte>();
        public string MimeType { get; init; } = "";
    }
}