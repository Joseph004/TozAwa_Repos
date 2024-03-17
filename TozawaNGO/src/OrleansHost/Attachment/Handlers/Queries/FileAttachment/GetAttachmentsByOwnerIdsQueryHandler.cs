using MediatR;
using Grains.Attachment.Models.Dtos;
using OrleansHost.Attachment.Models.Queries;

namespace OrleansHost.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsByOwnerIdsQueryHandler(IMediator mediator) : IRequestHandler<GetAttachmentsByOwnerIdsQuery, List<OwnerAttachments>>
{
    private readonly IMediator _mediator = mediator;
    public async Task<List<OwnerAttachments>> Handle(GetAttachmentsByOwnerIdsQuery request, CancellationToken cancellationToken)
    {
        var result = new List<OwnerAttachments>();

        foreach (var item in request.OwnerIds)
        {
            var response = (await _mediator.Send(new GetAttachmentsQuery { OwnerId = item }, cancellationToken)).ToList();

            result.Add(new OwnerAttachments
            {
                OwnerId = item,
                Attachments = response.Select(x => new Grains.Models.Dtos.FileAttachmentDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    ModifiedBy = x.ModifiedBy,
                    CreatedBy = x.CreatedBy,
                    OwnerIds = x.OwnerIds,
                    BlobId = x.BlobId,
                    MiniatureId = x.MiniatureId,
                    Name = x.Name,
                    Extension = x.Extension,
                    MimeType = x.MimeType,
                    Size = x.Size,
                    MetaData = x.MetaData,
                    Thumbnail = x.Thumbnail,
                    MiniatureBlobUrl = x.MiniatureBlobUrl,
                    FileAttachmentType = x.FileAttachmentType
                }).ToList()
            });
        }

        var groupedOwnerFileAttachments = result.GroupBy(ofa => ofa.OwnerId);

        return groupedOwnerFileAttachments
            .Select(x =>
                new OwnerAttachments
                {
                    OwnerId = x.Key,
                    Attachments = x.SelectMany(y => y.Attachments).ToList()
                }).ToList();
    }
}
