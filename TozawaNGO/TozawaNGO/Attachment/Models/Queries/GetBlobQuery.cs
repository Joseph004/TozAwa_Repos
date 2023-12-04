using MediatR;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetBlobQuery : IRequest<byte[]>
    {
        public Guid Id { get; set; }
        public GetBlobQuery(Guid id)
        {
            Id = id;
        }
    }
}
