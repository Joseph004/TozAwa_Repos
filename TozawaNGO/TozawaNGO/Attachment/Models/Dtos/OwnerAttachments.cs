namespace TozawaNGO.Attachment.Models.Dtos
{
    public class OwnerAttachments
    {
        public Guid OwnerId { get; set; }
        public List<TozawaNGO.Models.Dtos.FileAttachmentDto> Attachments { get; set; } = new List<TozawaNGO.Models.Dtos.FileAttachmentDto>();
    }
}