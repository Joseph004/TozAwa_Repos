using FluentValidation;
using OrleansHost.Attachment.Models.Commands;


namespace OrleansHost.Attachment.Models.Validation
{
    
    public class ConvertImageToPngCommandValidator : AbstractValidator<ConvertImageToPngCommand>
    {
        public ConvertImageToPngCommandValidator()
        {
            RuleFor(x => x.Bytes).NotEmpty();
        }
    }
}