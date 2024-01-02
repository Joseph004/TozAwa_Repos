using FluentValidation;
using MediatR;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetAttachmentQuery : IRequest<AttachmentDownloadDto>
    {
        public GetAttachmentQuery(Guid id) => Id = id;
        public Guid Id { get; set; }
    }

    public class GetAttachmentQueryValidator : AbstractValidator<GetAttachmentQuery>
    {
        public GetAttachmentQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}