using MediatR;

namespace Tozawa.Attachment.Svc.Models.Commands
{
    public class DeleteBlobCommand : IRequest
    {
        public DeleteBlobCommand(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }
    }
}
