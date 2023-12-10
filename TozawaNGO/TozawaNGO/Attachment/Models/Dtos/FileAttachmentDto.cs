
namespace TozawaNGO.Attachment.Models.Dtos
{
    public class FileAttachmentDto : BaseDto
    {
        public string BlobId { get; init; }
        public string Extension { get; set; } = "";
        public string MimeType { get; set; } = "";
        public double Size { get; set; }
        public string Name { get; set; }
        public string FileAttachmentType { get; set; }
        public string MetaData { get; set; } = "";
        public List<Guid> OwnerIds { get; set; } = new List<Guid>();
        public string MiniatureId { get; set; }
        public string BlobUrl { get; set; } = "";
        public string ImageClass { get; set; } = "";
        public string MiniatureBlobUrl { get; set; } = "";
    }
}