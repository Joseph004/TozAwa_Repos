using System;

namespace Tozawa.Bff.Portal.Models.Dtos
{
    public class OwnerAttachments
    {
        public Guid OwnerId { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; } = new List<FileAttachmentDto>();
    }
}