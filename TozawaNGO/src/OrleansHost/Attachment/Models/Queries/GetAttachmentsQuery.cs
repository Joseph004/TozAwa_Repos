using MediatR;
using OrleansHost.Attachment.Models.Dtos;
using OrleansHost.Models.Enums;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsQuery : IRequest<IEnumerable<OrleansHost.Models.Dtos.FileAttachmentDto>>
    {
        public Guid OwnerId { get; set; }
        public AttachmentType? FileAttachmentType { get; set; } = null;
    }
}