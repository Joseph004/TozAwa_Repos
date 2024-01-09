namespace TozawaNGO.Auth.Models.Dtos.Backend
{
    public class MemberDto : CurrentUserDto
    {
        public string Description { get; set; }
        public bool Deleted { get; set; }
        public bool DeletedForever { get; set; }
        public Guid DescriptionTextId { get; set; }
        public string Thumbnail { get; set; } = "";
        public List<TozawaNGO.Models.Dtos.FileAttachmentDto> Attachments { get; set; } = [];
    }
}


