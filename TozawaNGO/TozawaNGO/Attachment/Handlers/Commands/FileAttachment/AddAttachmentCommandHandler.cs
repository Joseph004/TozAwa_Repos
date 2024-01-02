using System.Net;
using MediatR;
using TozawaNGO.Attachment.Converters;
using TozawaNGO.Attachment.Models.Commands;
using TozawaNGO.Context;
using TozawaNGO.Helpers;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Models.ResponseRequests;
using TozawaNGO.Services;

namespace TozawaNGO.Attachment.Handlers.Commands;

public class AddAttachmentCommandHandler : IRequestHandler<AddAttachmentCommand, AddResponse<IEnumerable<TozawaNGO.Models.Dtos.FileAttachmentDto>>>
{
    private readonly TozawangoDbContext _context;
    private readonly IFileAttachmentCreator _fileAttachmentCreator;
    private readonly IGoogleService _googleService;
    private readonly IFileAttachmentConverter _fileAttachmentConverter;
    private readonly ILogger<AddAttachmentCommandHandler> _logger;

    public AddAttachmentCommandHandler(TozawangoDbContext context, IGoogleService googleService,
        IFileAttachmentCreator fileAttachmentCreator,
        IFileAttachmentConverter fileAttachmentConverter,
        ILogger<AddAttachmentCommandHandler> logger)
    {
        _context = context;
        _googleService = googleService;
        _fileAttachmentCreator = fileAttachmentCreator;
        _fileAttachmentConverter = fileAttachmentConverter;
        _logger = logger;
    }

    public async Task<AddResponse<IEnumerable<FileAttachmentDto>>> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var attachments = new List<FileAttachmentDto>();
            foreach (var requestFile in request.Files)
            {
                var blobId = await _googleService.UploadFile(FileUtil.StreamFromByteArray(requestFile.Content), requestFile.Name, requestFile.ContentType, request.FolderName, requestFile.Description);

                var isImage = FormFileConverter.IsImage(requestFile.ContentType, requestFile.Content);
                var extension = requestFile.Name.Split('.').Last();

                string miniatureId = null;

                if (isImage)
                {
                    requestFile.Content = ImageProcessingHelper.ResizeImageToTargetAmountOfPixels(requestFile.Content, 2481 * 3508, resizeIfSmaller: false);
                    miniatureId = await _googleService.UploadFile(FileUtil.StreamFromByteArray(ThumbnailProvider.GetThumbnail(requestFile.Content, extension)), requestFile.Name, requestFile.ContentType, request.FolderName, requestFile.Description);
                }

                IFormFile file = FileUtil.FileFromByteArray(requestFile.Name, requestFile.ContentType, requestFile.Content);

                var command = new AddAttachmentRequest
                {
                    OwnerIds = new List<Guid> { request.Id },
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
                await _context.SaveChangesAsync();

                if (attachment == null)
                {
                    return new AddResponse<IEnumerable<FileAttachmentDto>>(false, UpdateMessages.EntityCreatedError, HttpStatusCode.InternalServerError, null);
                }
                var converted = _fileAttachmentConverter.Convert(attachment);
                if (!string.IsNullOrEmpty(converted.MiniatureId))
                {
                    var stream = await _googleService.StreamFromGoogleFileByFolder(request.FolderName, converted.MiniatureId);
                    var bytes = FileUtil.ReadAllBytesFromStream(stream);
                    if (bytes != null)
                    {
                        converted.MiniatureBlobUrl = Convert.ToBase64String(bytes);
                    }
                }
                attachments.Add(converted);
            }
            return new AddResponse<IEnumerable<FileAttachmentDto>>(true, UpdateMessages.EntityCreatedSuccess, HttpStatusCode.OK, attachments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add attachement");
            return new AddResponse<IEnumerable<FileAttachmentDto>>(false, UpdateMessages.EntityCreatedError, HttpStatusCode.InternalServerError, null);
        }
    }
}
