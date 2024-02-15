using FluentValidation;
using TozawaNGO.Attachment.Models.Commands;

namespace TozawaNGO.Attachment.Models.Validation
{
    
    public class AddBlobCommandValidator : AbstractValidator<AddBlobCommand>
    {
        public AddBlobCommandValidator()
        {
            RuleFor(x => x.File).NotEmpty();
        }
    }
}