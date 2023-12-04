using FluentValidation;
using TozawaNGO.Attachment.Models.Commands;

namespace TozawaNGO.Attachment.Models.Validation
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