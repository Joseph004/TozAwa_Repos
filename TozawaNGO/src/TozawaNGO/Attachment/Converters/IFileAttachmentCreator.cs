
using TozawaNGO.Attachment.Models;
using TozawaNGO.Attachment.Models.Commands;

namespace TozawaNGO.Attachment.Converters
{
    public interface IFileAttachmentCreator
    {
        Task<FileAttachment> Create(AddAttachmentRequest addAttachmentCommand);
    }
}