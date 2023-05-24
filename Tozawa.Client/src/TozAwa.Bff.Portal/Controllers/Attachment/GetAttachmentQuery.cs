
using FluentValidation;
using MediatR;
using Tozawa.Bff.Portal.Models.Dtos;

namespace Tozawa.Bff.Portal.Controllers
{
    public class GetAttachmentQuery : IRequest<AttachmentDownloadDto>
    {
        public Guid Id { get; init; }
    }

    public class GetAttachmentQueryValidator : AbstractValidator<GetAttachmentQuery>
    {
        public GetAttachmentQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}