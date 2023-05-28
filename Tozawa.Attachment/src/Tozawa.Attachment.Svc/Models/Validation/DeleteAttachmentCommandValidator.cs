using FluentValidation;
using Tozawa.Attachment.Svc.Models.Queries;

namespace Tozawa.Attachment.Svc.Models.Validation
{
    
    public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
    {
        public DeleteAttachmentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}