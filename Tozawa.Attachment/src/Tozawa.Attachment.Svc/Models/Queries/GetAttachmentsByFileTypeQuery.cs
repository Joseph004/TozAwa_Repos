using MediatR;
using FluentValidation;
using Tozawa.Attachment.Svc.Models.Dtos;
using Tozawa.Attachment.Svc.Helpers;

namespace Tozawa.Attachment.Svc.Models.Queries
{
    public class GetAttachmentsByFileTypeQuery : IRequest<IEnumerable<FileAttachmentDto>>
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