using FluentValidation;


namespace Grains.Attachment.Models.Validation
{
    
    public class ConvertImageToPngCommandValidator : AbstractValidator<ConvertImageToPngCommand>
    {
        public ConvertImageToPngCommandValidator()
        {
            RuleFor(x => x.File).NotEmpty();
            RuleFor(x => x.File.ContentType).Must(x => x.Contains("image/"));
        }
    }
}