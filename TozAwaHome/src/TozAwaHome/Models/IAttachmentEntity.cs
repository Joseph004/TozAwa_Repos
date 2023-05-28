using System;
using System.Collections.Generic;
using TozAwaHome.Models.Dtos;

namespace TozAwaHome.Models
{
    public interface IAttachmentEntity
    {
        Guid Id { get; set; }
        string Code { get; set; }
        string Email { get; set; }
        bool Deleted { get; set; }
        List<FileAttachmentDto> Attachments { get; set; }
        string Thumbnail { get; set; }
    }
}