namespace Tozawa.Attachment.Svc.Context.Models
{
    public class OwnerFileAttachment
    {
        public Guid OwnerId { get; set; }
        public FileAttachment FileAttachment { get; set; }
        public Guid FileAttachmentId { get; set; }
    }
}