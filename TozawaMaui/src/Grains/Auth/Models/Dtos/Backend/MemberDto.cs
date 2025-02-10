namespace Grains.Auth.Models.Dtos.Backend
{
    public class MemberDto : CurrentUserDto
    {
        public string Description { get; set; }
        public Guid DescriptionTextId { get; set; }
        public string Comment { get; set; }
        public Guid CommentTextId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Deleted { get; set; }
        public bool DeletedForever { get; set; }
        public string Thumbnail { get; set; } = "";
        public List<Grains.Models.Dtos.FileAttachmentDto> Attachments { get; set; } = [];
        public int AttachmentsCount { get; set; }
    }
}


