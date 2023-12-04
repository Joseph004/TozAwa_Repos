
namespace TozawaNGO.Attachment.Models.Dtos
{
    public class FileAttachmentDto
    {
        public Guid Id { get; set; }
        public string BlobId { get; set; }
        public string MiniatureId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public double Size { get; set; }
        public IList<Guid> OwnerIds { get; set; }
        public string FileAttachmentType { get; set; }
        public string MetaData { get; set; }
        public Guid OrganizationId { get; set; }
    }
}