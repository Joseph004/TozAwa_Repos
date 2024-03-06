using Grains.Attachment.Models;
using Grains.Models.Dtos;

namespace OrleansHost.Attachment.Converters
{
    public interface IFileAttachmentConverter
    {
        FileAttachmentDto Convert(FileAttachment attachment);
    }
}