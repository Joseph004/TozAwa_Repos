using FluentValidation;
using OrleansHost.Attachment.Models.Queries;


namespace OrleansHost.Attachment.Models.Validation
{
    
    public class GetBlobQueryValidator : AbstractValidator<GetBlobQuery>
    {
        public GetBlobQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}