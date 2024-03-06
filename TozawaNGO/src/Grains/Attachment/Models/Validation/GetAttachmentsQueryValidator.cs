using FluentValidation;
using OrleansHost.Attachment.Models.Queries;

namespace Grains.Attachment.Models.Validation
{
    
    public class GetAttachmentsQueryValidator : AbstractValidator<GetAttachmentsQuery>
    {
        public GetAttachmentsQueryValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }
}