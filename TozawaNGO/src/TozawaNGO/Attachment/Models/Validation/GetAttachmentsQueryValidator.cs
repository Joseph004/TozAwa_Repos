using FluentValidation;
using TozawaNGO.Attachment.Models.Queries;

namespace TozawaNGO.Attachment.Models.Validation
{
    
    public class GetAttachmentsQueryValidator : AbstractValidator<GetAttachmentsQuery>
    {
        public GetAttachmentsQueryValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }
}