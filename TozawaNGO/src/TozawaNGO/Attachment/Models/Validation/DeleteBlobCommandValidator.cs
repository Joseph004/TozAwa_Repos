using FluentValidation;
using TozawaNGO.Attachment.Models.Commands;

namespace TozawaNGO.Attachment.Models.Validation
{
    
    public class DeleteBlobCommandValidator : AbstractValidator<DeleteBlobCommand>
    {
        public DeleteBlobCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}