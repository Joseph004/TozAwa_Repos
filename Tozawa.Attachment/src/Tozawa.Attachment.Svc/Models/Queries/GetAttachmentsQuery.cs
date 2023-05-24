using MediatR;
using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.Helpers;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Models.Queries
{
    public class GetAttachmentsQuery : IRequest<IEnumerable<FileAttachmentDto>>
    {
        public Guid OwnerId { get; set; }
        public AttachmentType? FileAttachmentType { get; set; } = null;
    }
}