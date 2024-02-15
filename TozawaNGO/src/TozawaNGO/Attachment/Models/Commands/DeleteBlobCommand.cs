using MediatR;

namespace TozawaNGO.Attachment.Models.Commands
{
    public class DeleteBlobCommand(Guid id) : IRequest
    {
        public Guid Id { get; set; } = id;
    }
}
