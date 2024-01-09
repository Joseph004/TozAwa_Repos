using TozawaNGO.Attachment.Models;
using TozawaNGO.Attachment.Models.Commands;
using TozawaNGO.Context;
using TozawaNGO.Extensions;
using TozawaNGO.Models.Enums;

namespace TozawaNGO.Attachment.Converters
{
    public class FileAttachmentCreator : IFileAttachmentCreator
    {
        private readonly TozawangoDbContext _TozawangoDbContext;

        public FileAttachmentCreator(TozawangoDbContext TozawangoDbContext)
        {
            _TozawangoDbContext = TozawangoDbContext;
        }
        public async Task<FileAttachment> Create(AddAttachmentRequest addAttachmentCommand)
        {
            await Task.FromResult(1);
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
