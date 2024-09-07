using FluentValidation;
using MediatR;
using Grains.Models.Dtos;

namespace OrleansHost.Attachment.Models.Queries
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