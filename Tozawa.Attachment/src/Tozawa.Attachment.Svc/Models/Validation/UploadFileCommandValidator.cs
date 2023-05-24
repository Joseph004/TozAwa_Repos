using FluentValidation;
using Tozawa.Attachment.Svc.Models.Commands;

namespace Tozawa.Attachment.Svc.Models.Validation
{
    
    public class AddBlobCommandValidator : AbstractValidator<AddBlobCommand>
    {
        public AddBlobCommandValidator()
        {
            RuleFor(x => x.File).NotEmpty();
        }
    }
}