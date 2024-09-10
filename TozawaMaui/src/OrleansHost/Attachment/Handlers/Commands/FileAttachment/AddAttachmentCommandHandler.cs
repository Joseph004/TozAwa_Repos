using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrleansHost.Attachment.Converters;
using OrleansHost.Attachment.Models.Commands;
using Grains.Context;
using Grains.Helpers;
using Grains.Models.Dtos;
using Grains.Models.ResponseRequests;
using Grains.Services;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;
using Grains;

namespace OrleansHost.Attachment.Handlers.Commands;

public class AddAttachmentCommandHandler(TozawangoDbContext context, IGoogleService googleService,
    IFileAttachmentCreator fileAttachmentCreator,
    IFileAttachmentConverter fileAttachmentConverter,
    IGrainFactory factory,
    IHubContext<ClientHub> hub,
    ILogger<AddAttachmentCommandHandler> logger) : IRequestHandler<AddAttachmentCommand, AddResponse<IEnumerable<Grains.Models.Dtos.FileAttachmentDto>>>
{
    private readonly IGrainFactory _factory = factory;
    private readonly IHubContext<ClientHub> _hub = hub;
    private readonly TozawangoDbContext _context = context;
    private readonly IFileAttachmentCreator _fileAttachmentCreator = fileAttachmentCreator;
    private readonly IGoogleService _googleService = googleService;
    private readonly IFileAttachmentConverter _fileAttachmentConverter = fileAttachmentConverter;
    private readonly ILogger<AddAttachmentCommandHandler> _logger = logger;

    public async Task<AddResponse<IEnumerable<FileAttachmentDto>>> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var attachments = new List<FileAttachmentDto>();
            foreach (var requestFile in request.Files)
            {
                var blobId = await _googleService.UploadFile(FileUtil.StreamFromByteArray(requestFile.Content), requestFile.Name, requestFile.ContentType, request.Id.ToString(), requestFile.Description);

                var isImage = FormFileConverter.IsImage(requestFile.ContentType, requestFile.Content);
                var extension = requestFile.Name.Split('.').Last();

                string miniatureId = null;

                if (isImage)
                {
                    requestFile.Content = ImageProcessingHelper.ResizeImageToTargetAmountOfPixels(requestFile.Content, 2481 * 3508, resizeIfSmaller: false);
                    miniatureId = await _googleService.UploadFile(FileUtil.StreamFromByteArray(ThumbnailProvider.GetThumbnail(requestFile.Content, extension)), requestFile.Name, requestFile.ContentType, request.Id.ToString(), requestFile.Description);
                }

                IFormFile file = FileUtil.FileFromByteArray(requestFile.Name, requestFile.ContentType, requestFile.Content);

                var command = new AddAttachmentRequest
                {
                    OwnerIds = [request.Id],
                    FileAttachmentType = request.FileAttachmentType,
                    BlobId = blobId,
                    Extension = extension,
                    MimeType = requestFile.ContentType,
                    MiniatureId = miniatureId,
                    Name = requestFile.Name,
                    Size = requestFile.Content.Length,

                };

                var attachment = await _fileAttachmentCreator.Create(command);

                _context.FileAttachments.Add(attachment);
                _context.SaveChanges();

                if (attachment == null)
                {
                    return new AddResponse<IEnumerable<FileAttachmentDto>>(false, UpdateMessages.EntityCreatedError, HttpStatusCode.InternalServerError, null);
                }
                var converted = _fileAttachmentConverter.Convert(attachment);
                if (!string.IsNullOrEmpty(converted.MiniatureId))
                {
                    var stream = await _googleService.StreamFromGoogleFileByFolder(request.Id.ToString(), converted.MiniatureId);
                    var bytes = FileUtil.ReadAllBytesFromStream(stream);
                    if (bytes != null)
                    {
                        converted.Thumbnail = Convert.ToBase64String(bytes);
                        converted.MiniatureBlobUrl = Convert.ToBase64String(bytes);
                    }
                }
                attachments.Add(converted);
                var item = new AttachmentItem(
                    converted.Id,
               converted.CreatedDate,
               converted.ModifiedDate,
               converted.ModifiedBy,
               converted.CreatedBy,
               SystemTextId.AttachmentOwnerId,
               converted.BlobId,
               converted.MiniatureId,
               converted.Name,
               converted.Extension,
               converted.MimeType,
               converted.Size,
               attachment.AttachmentType,
               attachment.FileAttachmentType,
               converted.MetaData,
               converted.OwnerIds,
               converted.Thumbnail,
               converted.MiniatureBlobUrl
                );
                await _factory.GetGrain<IAttachmentGrain>(item.Id).SetAsync(item);
            }
            await _hub.Clients.All.SendAsync("AttachmentAdded", string.Join(",", attachments.Select(x => x.Id)), request.Id, request.Source, cancellationToken: cancellationToken);
            return new AddResponse<IEnumerable<FileAttachmentDto>>(true, UpdateMessages.EntityCreatedSuccess, HttpStatusCode.OK, attachments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add attachement");
            return new AddResponse<IEnumerable<FileAttachmentDto>>(false, UpdateMessages.EntityCreatedError, HttpStatusCode.InternalServerError, null);
        }
    }
}
