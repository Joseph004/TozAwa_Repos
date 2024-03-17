using FluentValidation;
using MediatR;
using Grains.Models.ResponseRequests;

namespace OrleansHost.Attachment.Models.Queries;

public class DeleteAttachmentCommand : IRequest<DeleteResponse>
{
    public Guid Id { get; set; }
    public string Source { get; set; }
}
public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
{
    public DeleteAttachmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Source).NotNull().NotEmpty();
    }
}
