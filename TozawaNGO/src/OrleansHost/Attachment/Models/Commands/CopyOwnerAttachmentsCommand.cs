using FluentValidation;
using MediatR;

namespace OrleansHost.Attachment.Models.Commands
{
    public class CopyOwnerAttachmentsCommand : IRequest<int>
    {
        public Guid ToOwnerId { get; set; }
        public Guid FromOwnerId { get; set; }
    }

    public class CopyOwnerAttachmentsCommandValidator : AbstractValidator<CopyOwnerAttachmentsCommand>
    {
        public CopyOwnerAttachmentsCommandValidator()
        {
            RuleFor(x => x.FromOwnerId).NotEmpty();
            RuleFor(x => x.ToOwnerId).NotEmpty();
        }
    }
}
