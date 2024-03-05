
namespace OrleansHost.Models.Dtos
{
    public class OwnerAttachments
    {
        public Guid OwnerId { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; } = [];
    }
}


