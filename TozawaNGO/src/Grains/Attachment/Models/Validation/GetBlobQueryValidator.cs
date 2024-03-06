using FluentValidation;


namespace Grains.Attachment.Models.Validation
{
    
    public class GetBlobQueryValidator : AbstractValidator<GetBlobQuery>
    {
        public GetBlobQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}