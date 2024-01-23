using FluentValidation;
using MediatR;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Attachment.Models.Queries
{
    public class GetAttachmentQuery(Guid id) : IRequest<AttachmentDownloadDto>
    {
        public Guid Id { get; set; } = id;
    }

    public class GetAttachmentQueryValidator : AbstractValidator<GetAttachmentQuery>
    {
        public GetAttachmentQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}