
namespace ShareRazorClassLibrary.Models.Dtos
{
    public class MemberDto : CurrentUserDto, IAttachmentEntity, ITextEntity
    {
        public string Description { get; set; } = "";
        public string Comment { get; set; } = "";
        public string Text { get; set; } = "";
        public Guid DescriptionTextId { get; set; }
        public string Thumbnail { get; set; }
        public int AttachmentsCount { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; } = [];
    }
}


