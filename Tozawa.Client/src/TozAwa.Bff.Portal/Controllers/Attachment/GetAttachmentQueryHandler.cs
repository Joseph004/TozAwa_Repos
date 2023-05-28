
using MediatR;
using Tozawa.Bff.Portal.Helper;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    public class GetAttachmentQueryHandler : IRequestHandler<GetAttachmentQuery, AttachmentDownloadDto>
    {
        private readonly IAttachmentHttpClient _client;
        private readonly IGoogleService _googleService;
        private readonly ILogger<GetAttachmentQueryHandler> _logger;
        public GetAttachmentQueryHandler(IAttachmentHttpClient client, IGoogleService googleService, ILogger<GetAttachmentQueryHandler> logger)
        {
            _client = client;
            _googleService = googleService;
            _logger = logger;
        }

        public async Task<AttachmentDownloadDto> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _client.Get<FileAttachmentDto>($"fileattachment/{request.Id}");
                if (response == null)
                {
                    return new AttachmentDownloadDto();
                }

                var stream = await _googleService.StreamFromGoogleFileByFileId(response.BlobId);
                var bytes = FileUtil.ReadAllBytesFromStream(stream);

                var attchmentResponse = new AttachmentDownloadDto
                {
                    Content = bytes,
                    MimeType = response.MimeType,
                    Name = response.Name
                };
                return attchmentResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add attachement");
                return new AttachmentDownloadDto();
            }
        }
    }
}