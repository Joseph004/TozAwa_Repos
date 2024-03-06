using MediatR;
using Grains.Attachment.Models.Dtos;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsByOwnerIdsQuery : IRequest<List<OwnerAttachments>>
    {
        public List<Guid> OwnerIds { get; set; }
    }
}