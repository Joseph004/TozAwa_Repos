using MediatR;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Models.Queries
{
    public class GetAttachmentsByOwnerIdsQuery : IRequest<IEnumerable<TravlingFileAttachments>>
    {
        public List<Guid> OwnerIds { get; set; }
    }
}