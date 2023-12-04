using MediatR;
using TozawaNGO.Attachment.Models.Dtos;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetAttachmentsByOwnerIdsQuery : IRequest<IEnumerable<AnalyseFileAttachments>>
    {
        public List<Guid> OwnerIds { get; set; }
    }
}