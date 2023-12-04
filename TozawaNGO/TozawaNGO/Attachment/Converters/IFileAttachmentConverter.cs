using TozawaNGO.Attachment.Models;
using TozawaNGO.Attachment.Models.Dtos;

namespace TozawaNGO.Attachment.Converters
{
    public interface IFileAttachmentConverter
    {
        FileAttachmentDto Convert(FileAttachment attachment);
    }
}