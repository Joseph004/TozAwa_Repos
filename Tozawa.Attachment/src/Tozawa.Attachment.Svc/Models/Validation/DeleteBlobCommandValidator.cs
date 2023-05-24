using FluentValidation;
using Tozawa.Attachment.Svc.Models.Commands;

namespace Tozawa.Attachment.Svc.Models.Validation
{
    
    public class DeleteBlobCommandValidator : AbstractValidator<DeleteBlobCommand>
    {
        public DeleteBlobCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}