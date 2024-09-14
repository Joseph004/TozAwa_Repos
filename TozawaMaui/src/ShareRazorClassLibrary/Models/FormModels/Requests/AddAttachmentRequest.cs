using ShareRazorClassLibrary.Models.Enums;

namespace ShareRazorClassLibrary.Models.FormModels.Request
{
    public class AddAttachmentRequest
    {
        public string BlobId { get; set; }
        public string Extension { get; set; }
        public List<Guid> OwnerIds { get; set; }
        public string MimeType { get; set; }
        public string MiniatureId { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
        public string MetaData { get; set; }
    }
}