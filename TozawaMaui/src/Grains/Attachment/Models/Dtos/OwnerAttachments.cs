﻿namespace Grains.Attachment.Models.Dtos
{
    public class OwnerAttachments
    {
        public Guid OwnerId { get; set; }
        public List<Grains.Models.Dtos.FileAttachmentDto> Attachments { get; set; } = [];
    }
}