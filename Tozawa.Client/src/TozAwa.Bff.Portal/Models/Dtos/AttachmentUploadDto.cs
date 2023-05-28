namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class AttachmentUploadDto
    {
        public string Name { get; init; } = "";
        public string Description { get; set; } = "";
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; init; } = "";
    }
}