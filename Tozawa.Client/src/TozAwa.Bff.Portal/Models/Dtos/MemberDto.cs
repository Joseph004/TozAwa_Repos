using System;

namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class MemberDto : CurrentUserDto
    {
        public string Description { get; set; } = "";
        public Guid DescriptionTextId { get; set; }
        public string Thumbnail { get; set; } = "";
        public List<FileAttachmentDto> Attachments { get; set; } = new();
    }
}


