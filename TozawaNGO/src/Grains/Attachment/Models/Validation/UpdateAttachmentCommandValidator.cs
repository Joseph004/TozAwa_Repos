using FluentValidation;
using Grains.Attachment.Models.Commands;

namespace Grains.Attachment.Models.Validation
{
    
    public class UpdateAttachmentCommandValidator : AbstractValidator<UpdateAttachmentCommand>
    {
        public UpdateAttachmentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.File).NotEmpty();
            RuleFor(x => x.OwnerIds).NotEmpty();
        }
    }
}