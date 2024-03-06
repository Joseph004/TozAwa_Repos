using FluentValidation;
using OrleansHost.Attachment.Models.Queries;

namespace Grains.Attachment.Models.Validation
{
    
    public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
    {
        public DeleteAttachmentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}