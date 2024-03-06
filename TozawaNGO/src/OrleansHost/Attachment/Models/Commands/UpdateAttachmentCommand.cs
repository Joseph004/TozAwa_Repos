using MediatR;
using Microsoft.AspNetCore.Http;
using Grains.Attachment.Models.Dtos;
using Grains.Models.Enums;

namespace OrleansHost.Attachment.Models.Commands
{
    public class UpdateAttachmentCommand : IRequest<Grains.Models.Dtos.FileAttachmentDto>
    {
        public Guid Id { get; set; }
        public IFormFile File { get; set; }
        public List<Guid> OwnerIds { get; set; }
        public string MetaData { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
    }
}