using FluentValidation;
using MediatR;
using TozawaNGO.Models.ResponseRequests;

namespace TozawaNGO.Attachment.Models.Queries;

public class DeleteAttachmentCommand : IRequest<DeleteResponse>
{
    public Guid Id { get; set; }
}
public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
{
    public DeleteAttachmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
