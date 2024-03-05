namespace OrleansHost.Attachment.Models.Dtos
{
    public class OwnerAttachments
    {
        public Guid OwnerId { get; set; }
        public List<OrleansHost.Models.Dtos.FileAttachmentDto> Attachments { get; set; } = [];
    }
}