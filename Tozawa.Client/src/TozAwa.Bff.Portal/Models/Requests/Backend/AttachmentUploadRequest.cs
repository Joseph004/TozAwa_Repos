using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Models.Request.Backend
{
    public class AttachmentUploadRequest
    {
        public string BlobId { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public string MiniatureId { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public Guid FileAttachmentType { get; set; }
        public string MetaData { get; set; }
    }
}