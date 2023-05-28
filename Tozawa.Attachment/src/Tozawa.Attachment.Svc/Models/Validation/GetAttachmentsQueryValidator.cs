using FluentValidation;
using Tozawa.Attachment.Svc.Models.Queries;

namespace Tozawa.Attachment.Svc.Models.Validation
{
    
    public class GetAttachmentsQueryValidator : AbstractValidator<GetAttachmentsQuery>
    {
        public GetAttachmentsQueryValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }
}