using MediatR;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Models.Enums;

namespace TozawaNGO.Attachment.Models.Commands
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