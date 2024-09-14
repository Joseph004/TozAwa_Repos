using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Models
{
    public interface IAttachmentEntity
    {
        Guid Id { get; set; }
        string Email { get; set; }
        bool Deleted { get; set; }
        List<FileAttachmentDto> Attachments { get; set; }
        string Thumbnail { get; set; }
    }
}