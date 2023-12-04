namespace TozawaNGO.Attachment.Models.Dtos
{
    public class AnalyseFileAttachments
    {
        public Guid OwnerId { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; }
    }
}