using FluentValidation;
using Tozawa.Attachment.Svc.Models.Queries;

namespace Tozawa.Attachment.Svc.Models.Validation
{
    
    public class GetBlobQueryValidator : AbstractValidator<GetBlobQuery>
    {
        public GetBlobQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}