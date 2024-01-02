using MediatR;
using Microsoft.AspNetCore.Http;
using TozawaNGO.Attachment.Models.Dtos;
using TozawaNGO.Models.Enums;

namespace TozawaNGO.Attachment.Models.Commands
{
    public class UpdateAttachmentCommand : IRequest<TozawaNGO.Models.Dtos.FileAttachmentDto>
    {
        public Guid Id { get; set; }
        public IFormFile File { get; set; }
        public List<Guid> OwnerIds { get; set; }
        public string MetaData { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
    }
}