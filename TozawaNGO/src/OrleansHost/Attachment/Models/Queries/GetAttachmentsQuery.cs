using MediatR;
using Grains.Models.Enums;
using FluentValidation;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsQuery : IRequest<List<Grains.Models.Dtos.FileAttachmentDto>>
    {
        public Guid OwnerId { get; set; }
        public bool GetAll { get; set; } = false;
        public List<Guid> AttachmentIds { get; set; } = [];
        public AttachmentType? FileAttachmentType { get; set; } = null;
    }
}