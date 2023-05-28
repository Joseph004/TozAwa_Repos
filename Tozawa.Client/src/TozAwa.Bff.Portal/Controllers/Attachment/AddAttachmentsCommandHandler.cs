
using System.Net;
using MediatR;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Converters;
using Tozawa.Bff.Portal.Helper;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    public class AddAttachmentsCommandHandler : IRequestHandler<AddAttachmentsCommand, AddResponse<IEnumerable<FileAttachmentDto>>>
    {
        private readonly IAttachmentHttpClient _client;
        private readonly IGoogleService _googleService;
        private readonly ILogger<AddAttachmentsCommandHandler> _logger;
        public AddAttachmentsCommandHandler(IAttachmentHttpClient client, IGoogleService googleService, ILogger<AddAttachmentsCommandHandler> logger)
        {
            _client = client;
            _logger = logger;
            _googleService = googleService;
        }

        public async Task<AddResponse<IEnumerable<FileAttachmentDto>>> Handle(AddAttachmentsCommand request, CancellationToken cancellationToken)
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

                    var multiContent = new MultipartFormDataContent
                    {
                         {HttpClientHelperBase.CreateMultiPartContent(file), file.Name, file.FileName },
                         {HttpClientHelperBase.CreateHttpContent(new List<Guid> { request.Id }), "OwnerIds"},
                         {HttpClientHelperBase.CreateHttpContent(blobId), "BlobId"},
                         {HttpClientHelperBase.CreateHttpContent(extension), "extension"},
                         {HttpClientHelperBase.CreateHttpContent(requestFile.ContentType), "MimeType"},
                         {HttpClientHelperBase.CreateHttpContent(miniatureId), "MiniatureId"},
                         {HttpClientHelperBase.CreateHttpContent(requestFile.Name), "Name"},
                         {HttpClientHelperBase.CreateHttpContent(requestFile.Content.Length), "Size"},
                         {HttpClientHelperBase.CreateHttpContent(request.FileAttachmentType), "FileAttachmentType"}
                    };

                    var fileAttachment = await _client.Post<FileAttachmentDto>("fileattachment", multiContent, true);
                    if (fileAttachment == null)
                    {
                        return new AddResponse<IEnumerable<FileAttachmentDto>>(false, UpdateMessages.EntityCreatedError, HttpStatusCode.InternalServerError, null);
                    }

                    if (!string.IsNullOrEmpty(fileAttachment.MiniatureId))
                    {
                        var stream = await _googleService.StreamFromGoogleFileByFolder(request.FolderName, fileAttachment.MiniatureId);
                        var bytes = FileUtil.ReadAllBytesFromStream(stream);
                        if (bytes != null)
                        {
                            fileAttachment.MiniatureBlobUrl = Convert.ToBase64String(bytes);
                        }
                    }
                    attachments.Add(fileAttachment);
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
}