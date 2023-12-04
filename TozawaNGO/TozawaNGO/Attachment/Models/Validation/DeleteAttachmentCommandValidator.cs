using FluentValidation;
using TozawaNGO.Attachment.Models.Queries;

namespace TozawaNGO.Attachment.Models.Validation
{
    
    public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
    {
        public DeleteAttachmentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}