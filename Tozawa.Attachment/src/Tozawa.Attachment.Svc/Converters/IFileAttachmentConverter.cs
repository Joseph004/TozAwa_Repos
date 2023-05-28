using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Converters
{
    public interface IFileAttachmentConverter
    {
        FileAttachmentDto Convert(FileAttachment attachment);
    }
}