using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.Models.Commands;

namespace Tozawa.Attachment.Svc.Converters
{
    public interface IFileAttachmentCreator
    {
        Task<FileAttachment> Create(AddAttachmentCommand addAttachmentCommand);
    }
}