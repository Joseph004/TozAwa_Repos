using System;
using System.Collections.Generic;

namespace TozawaNGO.Models.Dtos
{
    public class MemberDto : CurrentUserDto, IAttachmentEntity
    {
        public string Description { get; set; } = "";
        public Guid DescriptionTextId { get; set; }
        public string Thumbnail { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; } = [];
    }
}


