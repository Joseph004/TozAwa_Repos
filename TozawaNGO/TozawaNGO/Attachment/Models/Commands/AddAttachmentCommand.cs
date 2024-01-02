using FluentValidation;
using MediatR;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.Enums;
using TozawaNGO.Models.ResponseRequests;

namespace TozawaNGO.Attachment.Models.Commands
{
    public class AddAttachmentCommand : IRequest<AddResponse<IEnumerable<TozawaNGO.Models.Dtos.FileAttachmentDto>>>
    {
        public Guid Id { get; set; }
        public string FolderName { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
        public List<AttachmentUploadDto> Files { get; set; } = new();
    }

    public class AddAttachmentsCommandValidator : AbstractValidator<AddAttachmentCommand>
    {
        public AddAttachmentsCommandValidator()
        {
            RuleFor(x => x.Files).NotEmpty().Must(a => a.All(y => FileValidator.IsValideFile(y)));
            RuleFor(x => x.FolderName).NotEmpty().Must(x => FileValidator.IsValidLength(x));
        }
    }

    public class AddAttachmentRequest
    {
        public string BlobId { get; set; }
        public string Extension { get; set; }
        public List<Guid> OwnerIds { get; set; }
        public string MimeType { get; set; }
        public string MiniatureId { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public AttachmentType? FileAttachmentType { get; set; }
        public string MetaData { get; set; }
    }
}