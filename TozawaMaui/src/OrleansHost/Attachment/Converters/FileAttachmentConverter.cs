﻿using Grains.Attachment.Models;
using Grains.Models.Dtos;

namespace OrleansHost.Attachment.Converters
{
    public class FileAttachmentConverter : IFileAttachmentConverter
    {
        public FileAttachmentDto Convert(FileAttachment attachment)
        {
            return new FileAttachmentDto
            {
                Id = attachment.Id,
                BlobId = attachment.BlobId,
                Extension = attachment.Extension,
                MimeType = attachment.MimeType,
                MiniatureId = attachment.MiniatureId,
                Name = attachment.Name,
                Size = attachment.Size,
                MetaData = attachment.MetaData,
                OwnerIds = attachment.Owners.Select(x => x.OwnerId).ToList(),
                FileAttachmentType = attachment.AttachmentType
            };
        }
    }
}
