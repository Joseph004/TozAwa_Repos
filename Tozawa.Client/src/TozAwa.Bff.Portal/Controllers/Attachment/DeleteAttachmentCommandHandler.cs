
using System.Net;
using MediatR;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Dtos;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;

namespace Tozawa.Bff.Portal.Controllers
{
    public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, DeleteResponse>
    {
        private readonly IAttachmentHttpClient _client;
        private readonly ILogger<DeleteAttachmentCommandHandler> _logger;
        private readonly IGoogleService _googleService;
        public DeleteAttachmentCommandHandler(IAttachmentHttpClient client, IGoogleService googleService, ILogger<DeleteAttachmentCommandHandler> logger)
        {
            _client = client;
            _googleService = googleService;
            _logger = logger;
        }

        public async Task<DeleteResponse> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var attachement = await _client.Get<FileAttachmentDto>($"fileattachment/{request.Id}");
                if (attachement == null)
                {
                    return new DeleteResponse(false, UpdateMessages.Error, HttpStatusCode.InternalServerError);
                }

                await _googleService.DeleteFile(attachement.BlobId);

                if (!string.IsNullOrEmpty(attachement.MiniatureId))
                {
                    await _googleService.DeleteFile(attachement.MiniatureId);
                }

                await _client.Delete($"fileattachment/{request.Id}", cancellationToken);
                return new DeleteResponse(true, UpdateMessages.EntityDeletedSuccess, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete attachement");
                return new DeleteResponse(false, UpdateMessages.Error, null);
            }
        }
    }
}