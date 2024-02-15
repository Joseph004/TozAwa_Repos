using MediatR;
using Microsoft.EntityFrameworkCore;
using TozawaNGO.Attachment.Models.Queries;
using TozawaNGO.Context;
using TozawaNGO.Services;
using TozawaNGO.Models.ResponseRequests;
using TozawaNGO.Helpers;
using System.Net;

namespace TozawaNGO.Attachment.Handlers.Commands;

public class DeleteAttachmentCommandHandler(TozawangoDbContext context, IGoogleService googleService, ILogger<DeleteAttachmentCommandHandler> logger) : IRequestHandler<DeleteAttachmentCommand, DeleteResponse>
{
    private readonly TozawangoDbContext _context = context;
    private readonly IGoogleService _googleService = googleService;
    private readonly ILogger<DeleteAttachmentCommandHandler> _logger = logger;

    public async Task<DeleteResponse> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var attachment = await _context.FileAttachments
                       .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken) ?? throw new Exception($"Attachment with Id [{request.Id}] was not found.");

            if (attachment == null)
            {
                return new DeleteResponse(false, UpdateMessages.Error, HttpStatusCode.InternalServerError);
            }

            await _googleService.DeleteFile(attachment.BlobId);
            if (!string.IsNullOrEmpty(attachment.MiniatureId))
            {
                await _googleService.DeleteFile(attachment.MiniatureId);
            }

            var ownerAttachment = await _context.OwnerFileAttachments.Where(x => x.FileAttachment.Id == attachment.Id).ToListAsync(cancellationToken: cancellationToken);
            _context.OwnerFileAttachments.RemoveRange(ownerAttachment);
            _context.FileAttachments.Remove(attachment);
            await _context.SaveChangesAsync(cancellationToken);
            return new DeleteResponse(true, UpdateMessages.EntityDeletedSuccess, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete attachement");
            return new DeleteResponse(false, UpdateMessages.Error, null);
        }
    }
}
