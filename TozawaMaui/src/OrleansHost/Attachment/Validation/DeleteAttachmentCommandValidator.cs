using FluentValidation;
using OrleansHost.Attachment.Models.Queries;

namespace OrleansHost.Attachment.Models.Validation
{
    
    public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
    {
        public DeleteAttachmentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}