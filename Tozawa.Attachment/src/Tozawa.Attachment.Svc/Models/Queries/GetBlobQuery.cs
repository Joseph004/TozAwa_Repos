using MediatR;

namespace Tozawa.Attachment.Svc.Models.Queries
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
