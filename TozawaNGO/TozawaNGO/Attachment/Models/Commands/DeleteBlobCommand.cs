using MediatR;

namespace TozawaNGO.Attachment.Models.Commands
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
