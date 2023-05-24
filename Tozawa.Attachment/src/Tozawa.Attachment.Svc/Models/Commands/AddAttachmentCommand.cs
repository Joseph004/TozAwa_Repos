using MediatR;
using Microsoft.AspNetCore.Http;
using Tozawa.Attachment.Svc.Context.Models;
using Tozawa.Attachment.Svc.Helpers;
using Tozawa.Attachment.Svc.Models.Dtos;

namespace Tozawa.Attachment.Svc.Models.Commands
{
    public class AddAttachmentCommand : IRequest<FileAttachmentDto>
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