
namespace TozawaMauiHybrid.Models.Dtos
{
    public class MemberDto : CurrentUserDto, IAttachmentEntity, ITextEntity
    {
        public string Description { get; set; } = "";
        public string Text { get; set; } = "";
        public Guid DescriptionTextId { get; set; }
        public string Thumbnail { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; } = [];
    }
}


