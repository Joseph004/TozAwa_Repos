using FluentValidation;


namespace Grains.Attachment.Models.Validation
{
    
    public class DeleteBlobCommandValidator : AbstractValidator<DeleteBlobCommand>
    {
        public DeleteBlobCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}