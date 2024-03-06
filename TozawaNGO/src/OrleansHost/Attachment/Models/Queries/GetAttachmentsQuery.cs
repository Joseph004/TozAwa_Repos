using MediatR;
using Grains.Attachment.Models.Dtos;
using Grains.Models.Enums;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsQuery : IRequest<IEnumerable<Grains.Models.Dtos.FileAttachmentDto>>
    {
        public Guid OwnerId { get; set; }
        public AttachmentType? FileAttachmentType { get; set; } = null;
    }
}