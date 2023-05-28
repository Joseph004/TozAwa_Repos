using MediatR;
using Microsoft.AspNetCore.Http;
using Tozawa.Attachment.Svc.Helpers;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Models.Commands
{
    public class UpdateAttachmentCommand : IRequest<FileAttachmentDto>
    {
        public Guid Id { get; set; }
        public IFormFile File { get; set; }
        public List<Guid> OwnerIds { get; set; }
        public string MetaData { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
    }
}