using Grains.Models.Dtos;
using MediatR;
using OrleansHost.Attachment.Converters;
using OrleansHost.Attachment.Models.Commands;

namespace OrleansHost.Attachment.Handlers.Commands
{
    public class ConvertImageToPngCommandHandler : IRequestHandler<ConvertImageToPngCommand, AttachmentDownloadDto>
    {
        public async Task<AttachmentDownloadDto> Handle(ConvertImageToPngCommand request, CancellationToken cancellationToken)
        {
            return new AttachmentDownloadDto
            {
                Content = await FormFileConverter.ImageToPng(request.Bytes)
            };
        }
    }
}