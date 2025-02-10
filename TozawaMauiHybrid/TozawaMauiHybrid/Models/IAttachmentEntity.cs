using TozawaMauiHybrid.Models.Dtos;

namespace TozawaMauiHybrid.Models
{
    public interface IAttachmentEntity
    {
        Guid Id { get; set; }
        string Email { get; set; }
        bool Deleted { get; set; }
        int AttachmentsCount { get; set; }
        List<FileAttachmentDto> Attachments { get; set; }
        string Thumbnail { get; set; }
    }
}