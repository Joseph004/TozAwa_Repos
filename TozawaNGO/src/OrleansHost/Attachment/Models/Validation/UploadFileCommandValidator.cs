using FluentValidation;
using OrleansHost.Attachment.Models.Commands;

namespace OrleansHost.Attachment.Models.Validation
{
    
    public class AddBlobCommandValidator : AbstractValidator<AddBlobCommand>
    {
        public AddBlobCommandValidator()
        {
            RuleFor(x => x.File).NotEmpty();
        }
    }
}