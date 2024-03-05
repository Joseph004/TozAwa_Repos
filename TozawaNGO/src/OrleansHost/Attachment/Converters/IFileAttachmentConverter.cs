using OrleansHost.Attachment.Models;
using OrleansHost.Models.Dtos;

namespace OrleansHost.Attachment.Converters
{
    public interface IFileAttachmentConverter
    {
        FileAttachmentDto Convert(FileAttachment attachment);
    }
}