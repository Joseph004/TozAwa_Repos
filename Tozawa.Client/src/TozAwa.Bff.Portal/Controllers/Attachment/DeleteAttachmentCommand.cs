
using FluentValidation;
using MediatR;
using Tozawa.Bff.Portal.Models.ResponseRequests;

namespace Tozawa.Bff.Portal.Controllers
{
    public class DeleteAttachmentCommand : IRequest<DeleteResponse>
    {
        public Guid Id { get; init; }
    }

    public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
    {
        public DeleteAttachmentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}