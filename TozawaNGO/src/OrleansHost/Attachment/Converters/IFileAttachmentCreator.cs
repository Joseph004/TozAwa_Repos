
using OrleansHost.Attachment.Models;
using OrleansHost.Attachment.Models.Commands;

namespace OrleansHost.Attachment.Converters
{
    public interface IFileAttachmentCreator
    {
        Task<FileAttachment> Create(AddAttachmentRequest addAttachmentCommand);
    }
}