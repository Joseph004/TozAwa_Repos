using MediatR;
using TozawaNGO.Attachment.Models.Dtos;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetAttachmentQuery : IRequest<FileAttachmentDto>
    {
        public GetAttachmentQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}