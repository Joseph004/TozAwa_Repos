﻿using MediatR;
using OrleansHost.Attachment.Models.Queries;
using System.Collections.Immutable;
using Grains;
using System.Buffers;
using Grains.Helpers;
using MudBlazor;

namespace OrleansHost.Attachment.Handlers.Queries.FileAttachment;

public class GetAttachmentsQueryHandler(
    IGrainFactory factory, IMediator mediator) : IRequestHandler<GetAttachmentsQuery, TableData<Grains.Models.Dtos.FileAttachmentDto>>
{
    public readonly IMediator _mediator = mediator;
    private readonly IGrainFactory _factory = factory;

    public async Task<TableData<Grains.Models.Dtos.FileAttachmentDto>> Handle(GetAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var keys = ImmutableArray.Create<Guid>(Guid.Empty);

        if (request.AttachmentIds.Count > 0)
        {
            var result = new List<AttachmentItem>();
            foreach (var item in request.AttachmentIds)
            {
                result.Add(await _factory.GetGrain<IAttachmentGrain>(item).GetAsync());
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
            if (response == null || response.Count == 0)
            {
                return new TableData<Grains.Models.Dtos.FileAttachmentDto>() { TotalItems = 0, Items = [] };
            }
            return new TableData<Grains.Models.Dtos.FileAttachmentDto>() { TotalItems = response.Count, Items = response };
        }
        else
        { 
            // get all item keys for this owner
            if (request.GetAll)
            {
                keys = await _factory.GetGrain<IAttachmentManagerGrain>(SystemTextId.AttachmentOwnerId).GetAllAsync();
            }
            else if (request.OwnerId != Guid.Empty)
            {
                keys = await _factory.GetGrain<IAttachmentManagerGrain>(SystemTextId.AttachmentOwnerId).GetAllByOwnerIdAsync(request.OwnerId);
            }
        }

        // fast path for empty owner
        if (keys.Length == 0 || keys.First() == Guid.Empty) new TableData<Grains.Models.Dtos.FileAttachmentDto>() { TotalItems = 0, Items = [] };

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
            if (response == null || response.Count == 0)
            {
                return new TableData<Grains.Models.Dtos.FileAttachmentDto>() { TotalItems = 0, Items = [] };
            }
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                response = response.Where(Filtered(request.SearchString)).ToList();
            }
            if (request.GetAll)
            {
                return new TableData<Grains.Models.Dtos.FileAttachmentDto>() { TotalItems = response.Count, Items = response };
            }
            var paged = response.ToList();
            if (request.Page.HasValue && request.PageSize.HasValue)
            {
                paged = response.Skip(request.Page.Value * request.PageSize.Value).Take(request.PageSize.Value).ToList();
            }
            return new TableData<Grains.Models.Dtos.FileAttachmentDto>() { TotalItems = response.Count, Items = paged.Where(x => x.OwnerIds.Count > 0 && x.OwnerIds.First() == request.OwnerId).ToList() };
        }
        finally
        {
            ArrayPool<Task<AttachmentItem>>.Shared.Return(tasks);
        }
    }
    private static Func<Grains.Models.Dtos.FileAttachmentDto, bool> Filtered(string searchString) => x =>
                                                                              (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                               (!string.IsNullOrEmpty(x.FileAttachmentType) && x.FileAttachmentType.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                                                               (!string.IsNullOrEmpty(x.Size.ToString()) && x.Size.ToString().Contains(searchString, StringComparison.InvariantCultureIgnoreCase));
}
