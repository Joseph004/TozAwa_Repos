using Grains.Models.Dtos;
using MediatR;

namespace OrleansHost.Attachment.Models.Commands
{
    public class ConvertImageToPngCommand(byte[] bytes) : IRequest<AttachmentDownloadDto>
    {
        public byte[] Bytes { get; set; } = bytes;
    }
}