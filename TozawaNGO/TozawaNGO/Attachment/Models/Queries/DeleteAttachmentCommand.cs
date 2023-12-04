using MediatR;

namespace TozawaNGO.Attachment.Models.Queries;

public class DeleteAttachmentCommand : IRequest
{
    public Guid Id { get; set; }
}
