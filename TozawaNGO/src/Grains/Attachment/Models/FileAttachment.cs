using Grains.Helpers;
using Grains.Models.Enums;

namespace Grains.Attachment.Models
{
    public class FileAttachment : BaseEntity
    {
        public string BlobId { get; set; }
        public string MiniatureId { get; set; } = null;
        public string Name { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public double Size { get; set; }
        public string AttachmentType { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
        public string MetaData { get; set; }

        public List<OwnerFileAttachment> Owners { get; set; }

        public FileAttachment Clone()
        {
            return (FileAttachment)MemberwiseClone();
        }
    }
}