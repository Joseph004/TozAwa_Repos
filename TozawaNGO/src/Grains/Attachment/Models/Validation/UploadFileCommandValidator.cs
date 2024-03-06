using FluentValidation;


namespace Grains.Attachment.Models.Validation
{
    
    public class AddBlobCommandValidator : AbstractValidator<AddBlobCommand>
    {
        public AddBlobCommandValidator()
        {
            RuleFor(x => x.File).NotEmpty();
        }
    }
}