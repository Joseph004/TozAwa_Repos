using FluentValidation;
using TozawaNGO.Attachment.Models.Commands;

namespace TozawaNGO.Attachment.Models.Validation
{
    
    public class AddAttachmentCommandValidator : AbstractValidator<AddAttachmentCommand>
    {
        public AddAttachmentCommandValidator()
        {
            RuleFor(x => x.OwnerIds).NotEmpty();
        }
    }
}