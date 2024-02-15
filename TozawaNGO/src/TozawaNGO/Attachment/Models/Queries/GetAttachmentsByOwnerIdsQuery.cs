using MediatR;
using TozawaNGO.Attachment.Models.Dtos;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetAttachmentsByOwnerIdsQuery : IRequest<List<OwnerAttachments>>
    {
        public List<Guid> OwnerIds { get; set; }
    }
}