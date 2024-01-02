using TozawaNGO.Attachment.Models;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Attachment.Converters
{
    public interface IFileAttachmentConverter
    {
        FileAttachmentDto Convert(FileAttachment attachment);
    }
}