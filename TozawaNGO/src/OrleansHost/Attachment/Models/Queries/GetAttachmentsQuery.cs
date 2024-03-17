using MediatR;
using Grains.Models.Enums;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsQuery : IRequest<List<Grains.Models.Dtos.FileAttachmentDto>>
    {
        public Guid OwnerId { get; set; }
        public AttachmentType? FileAttachmentType { get; set; } = null;
    }
}