using FluentValidation;
using TozawaNGO.Attachment.Models.Commands;

namespace TozawaNGO.Attachment.Models.Validation
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