using MediatR;

namespace Tozawa.Attachment.Svc.Models.Queries;

public class DeleteAttachmentCommand : IRequest
{
    public Guid Id { get; set; }
}
