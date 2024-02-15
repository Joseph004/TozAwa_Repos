using FluentValidation;
using TozawaNGO.Attachment.Models.Queries;

namespace TozawaNGO.Attachment.Models.Validation
{
    
    public class GetBlobQueryValidator : AbstractValidator<GetBlobQuery>
    {
        public GetBlobQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}