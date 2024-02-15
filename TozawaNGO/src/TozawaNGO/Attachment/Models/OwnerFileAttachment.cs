namespace TozawaNGO.Attachment.Models
{
    public class OwnerFileAttachment
    {
        public Guid OwnerId { get; set; }
        public FileAttachment FileAttachment { get; set; }
        public Guid FileAttachmentId { get; set; }
    }
}