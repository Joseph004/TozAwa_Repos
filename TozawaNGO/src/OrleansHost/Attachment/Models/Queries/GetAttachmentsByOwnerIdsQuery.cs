using MediatR;
using Grains.Attachment.Models.Dtos;
using FluentValidation;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsByOwnerIdsQuery : IRequest<List<OwnerAttachments>>
    {
        public List<Guid> OwnerIds { get; set; }
    }

    public class GetAttachmentsByOwnerIdsQueryValidator : AbstractValidator<GetAttachmentsByOwnerIdsQuery>
    {
        public GetAttachmentsByOwnerIdsQueryValidator()
        {
            RuleFor(x => x.OwnerIds).NotNull().NotEmpty();
        }
    }
}