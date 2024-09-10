using MediatR;

namespace OrleansHost.Attachment.Models.Commands
{
    public class DeleteBlobCommand(Guid id) : MediatR.IRequest
    {
        public Guid Id { get; set; } = id;
    }
}
