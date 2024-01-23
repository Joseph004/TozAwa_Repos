using MediatR;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetBlobQuery(Guid id) : IRequest<byte[]>
    {
        public Guid Id { get; set; } = id;
    }
}
