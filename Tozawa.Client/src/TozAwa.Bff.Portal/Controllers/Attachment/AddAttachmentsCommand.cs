
using FluentValidation;
using MediatR;
using Tozawa.Bff.Portal.Helpers;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.Request.Frontend;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using TozAwa.Bff.Portal.Helper;

namespace Tozawa.Bff.Portal.Controllers
{
    public class AddAttachmentsCommand : AttachmentUploadRequest, IRequest<AddResponse<IEnumerable<FileAttachmentDto>>>
    {
        public Guid Id { get; set; }
        public string FolderName { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
    }

    public class AddAttachmentsCommandValidator : AbstractValidator<AddAttachmentsCommand>
    {
        public AddAttachmentsCommandValidator()
        {
            RuleFor(x => x.Files).NotEmpty().Must(a => a.All(y => FileValidator.IsValideFile(y)));
            RuleFor(x => x.FolderName).NotEmpty().Must(x => FileValidator.IsValidLength(x));
        }
    }
}