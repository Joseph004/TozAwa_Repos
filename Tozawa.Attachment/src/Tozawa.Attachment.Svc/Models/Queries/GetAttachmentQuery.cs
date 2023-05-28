using MediatR;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Models.Queries
{
    public class GetAttachmentQuery : IRequest<FileAttachmentDto>
    {
        public GetAttachmentQuery(Guid id) => Id = id;

        public Guid Id { get; set; }
    }
}