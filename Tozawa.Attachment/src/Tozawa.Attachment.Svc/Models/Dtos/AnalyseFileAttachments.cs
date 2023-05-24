namespace Tozawa.Attachment.Svc.Models.Dtos
{
    public class TravlingFileAttachments
    {
        public Guid OwnerId { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; }
    }
}