using FluentValidation;
using OrleansHost.Attachment.Models.Commands;


namespace OrleansHost.Attachment.Models.Validation
{
    
    public class DeleteBlobCommandValidator : AbstractValidator<DeleteBlobCommand>
    {
        public DeleteBlobCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}