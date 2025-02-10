using MediatR;
using Grains.Models.Enums;
using MudBlazor;

namespace OrleansHost.Attachment.Models.Queries
{
    public class GetAttachmentsQuery : IRequest<TableData<Grains.Models.Dtos.FileAttachmentDto>>
    {
        public string SearchString { get; set; } = null;
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public Guid OwnerId { get; set; }
        public bool GetAll { get; set; } = false;
        public List<Guid> AttachmentIds { get; set; } = [];
        public AttachmentType? FileAttachmentType { get; set; } = null;
    }
}