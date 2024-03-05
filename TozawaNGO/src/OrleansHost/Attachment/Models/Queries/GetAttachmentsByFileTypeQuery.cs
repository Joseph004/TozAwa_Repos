using MediatR;
using FluentValidation;
using OrleansHost.Attachment.Models.Dtos;
using OrleansHost.Models.Enums;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsByFileTypeQuery : IRequest<IEnumerable<OrleansHost.Models.Dtos.FileAttachmentDto>>
    {
        public Guid OwnerId { get; set; }
        public AttachmentType AttachementType { get; set; }
    }

    public class GetAttachmentsByFileTypeQueryValidator : AbstractValidator<GetAttachmentsByFileTypeQuery>
    {
        public GetAttachmentsByFileTypeQueryValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.AttachementType).NotEmpty();
        }
    }
}