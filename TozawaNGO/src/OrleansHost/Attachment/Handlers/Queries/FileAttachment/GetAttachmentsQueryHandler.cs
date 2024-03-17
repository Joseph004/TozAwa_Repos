using MediatR;
using OrleansHost.Attachment.Models.Queries;
using System.Collections.Immutable;
using Grains;
using System.Buffers;
using Grains.Helpers;

namespace OrleansHost.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsQueryHandler(
    IGrainFactory factory) : IRequestHandler<GetAttachmentsQuery, List<Grains.Models.Dtos.FileAttachmentDto>>
{
    private readonly IGrainFactory _factory = factory;

    public async Task<List<Grains.Models.Dtos.FileAttachmentDto>> Handle(GetAttachmentsQuery request, CancellationToken cancellationToken)
    {
        // get all item keys for this owner
        var keys = await _factory.GetGrain<IAttachmentManagerGrain>(SystemTextId.AttachmentOwnerId).GetAllAsync();

        // fast path for empty owner
        if (keys.Length == 0) return [];

        // fan out and get all individual items in parallel
        var tasks = ArrayPool<Task<AttachmentItem>>.Shared.Rent(keys.Length);
        try
        {
            // issue all requests at the same time
            for (var i = 0; i < keys.Length; ++i)
            {
                tasks[i] = _factory.GetGrain<IAttachmentGrain>(keys[i]).GetAsync();
            }

            // compose the result as requests complete
            var result = ImmutableArray.CreateBuilder<AttachmentItem>(tasks.Length);
            for (var i = 0; i < keys.Length; ++i)
            {
                result.Add(await tasks[i]);
            }

            var response = new List<Grains.Models.Dtos.FileAttachmentDto>();
            foreach (var item in result)
            {
                response.Add(new Grains.Models.Dtos.FileAttachmentDto
                {
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate,
                    ModifiedBy = item.ModifiedBy,
                    CreatedBy = item.CreatedBy,
                    OwnerIds = item.OwnerIds,
                    BlobId = item.BlobId,
                    MiniatureId = item.MiniatureId,
                    Name = item.Name,
                    Extension = item.Extension,
                    MimeType = item.MimeType,
                    Size = item.Size,
                    MetaData = item.MetaData,
                    Thumbnail = item.Thumbnail,
                    MiniatureBlobUrl = item.MiniatureBlobUrl,
                    FileAttachmentType = item.AttachmentType
                });
            }
            return (response ?? []).Where(x => x.OwnerIds.First() == request.OwnerId).ToList();
        }
        finally
        {
            ArrayPool<Task<AttachmentItem>>.Shared.Return(tasks);
        }
    }
}
