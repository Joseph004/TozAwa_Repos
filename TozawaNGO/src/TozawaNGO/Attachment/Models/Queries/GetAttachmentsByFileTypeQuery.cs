using MediatR;
using FluentValidation;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Models.Enums;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetAttachmentsByFileTypeQuery : IRequest<IEnumerable<TozawaNGO.Models.Dtos.FileAttachmentDto>>
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