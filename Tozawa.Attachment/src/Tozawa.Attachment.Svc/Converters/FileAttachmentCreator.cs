using Tozawa.Attachment.Svc.Context;
using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.extension;
using Tozawa.Attachment.Svc.Helpers;
using Tozawa.Attachment.Svc.Models.Commands;

namespace Tozawa.Attachment.Svc.Converters
{
    public class FileAttachmentCreator : IFileAttachmentCreator
    {
        private readonly AttachmentContext _attachmentContext;

        public FileAttachmentCreator(AttachmentContext attachmentContext)
        {
            _attachmentContext = attachmentContext;
        }
        public async Task<FileAttachment> Create(AddAttachmentCommand addAttachmentCommand)
        {
            var fileAttachmentId = Guid.NewGuid();
            var fileAttachment = new FileAttachment
            {
                Id = fileAttachmentId,
                BlobId = addAttachmentCommand.BlobId,
                Extension = addAttachmentCommand.Extension,
                MimeType = addAttachmentCommand.MimeType,
                MiniatureId = addAttachmentCommand.MiniatureId,
                Name = addAttachmentCommand.Name,
                Size = addAttachmentCommand.Size,
                AttachmentType = addAttachmentCommand.FileAttachmentType.HasValue ? addAttachmentCommand.FileAttachmentType.ConvertToString() : AttachmentType.Intern.ConvertToString(),
                MetaData = addAttachmentCommand.MetaData
            };

            fileAttachment.Owners = addAttachmentCommand.OwnerIds.Select(x =>
                new OwnerFileAttachment
                {
                    OwnerId = x,
                    FileAttachmentId = fileAttachmentId,
                    FileAttachment = fileAttachment
                }
            ).ToList();

            return fileAttachment;
        }
    }
}
