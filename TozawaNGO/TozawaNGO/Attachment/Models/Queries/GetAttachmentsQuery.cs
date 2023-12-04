using MediatR;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Models.Enums;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetAttachmentsQuery : IRequest<IEnumerable<FileAttachmentDto>>
    {
        public Guid OwnerId { get; set; }
        public AttachmentType? FileAttachmentType { get; set; } = null;
    }
}