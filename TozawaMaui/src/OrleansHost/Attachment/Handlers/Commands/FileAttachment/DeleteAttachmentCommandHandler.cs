using MediatR;
using Microsoft.EntityFrameworkCore;
using OrleansHost.Attachment.Models.Queries;
using Grains.Context;
using Grains.Services;
using Grains.Models.ResponseRequests;
using Grains.Helpers;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Shared.SignalR;
using Grains;
using Grains.Auth.Models.Dtos.Backend;
using Grains.Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace OrleansHost.Attachment.Handlers.Commands;

public class DeleteAttachmentCommandHandler(TozawangoDbContext context, ICurrentUserService currentUserService, IGoogleService googleService, IGrainFactory factory,
    IHubContext<ClientHub> hub, ILogger<DeleteAttachmentCommandHandler> logger) : IRequestHandler<DeleteAttachmentCommand, DeleteResponse>
{
    private readonly IGrainFactory _factory = factory;
    private readonly IHubContext<ClientHub> _hub = hub;
    private readonly TozawangoDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;
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

            if (!_currentUserService.IsAdmin() && attachment.CreatedBy != _currentUserService.User.UserName)
            {
                throw new BadHttpRequestException("Cannot remove");
            }

            var fileId = attachment.Id;
            await _googleService.DeleteFile(attachment.BlobId);
            if (!string.IsNullOrEmpty(attachment.MiniatureId))
            {
                await _googleService.DeleteFile(attachment.MiniatureId);
            }

            var ownerAttachment = await _context.OwnerFileAttachments.Where(x => x.FileAttachment.Id == attachment.Id).ToListAsync(cancellationToken: cancellationToken);
            _context.OwnerFileAttachments.RemoveRange(ownerAttachment);
            _context.FileAttachments.Remove(attachment);
            _context.SaveChanges();

            await _factory.GetGrain<IAttachmentGrain>(fileId).ClearAsync();

            if (request.Source == nameof(MemberDto))
            {
                var memberItem = await _factory.GetGrain<IMemberGrain>(request.OwnerId).GetAsync();
                memberItem.AttachmentsCount--;
                await _factory.GetGrain<IMemberGrain>(request.OwnerId).SetAsync(memberItem);
            }

            await _hub.Clients.All.SendAsync("AttachmentDeleted", fileId.ToString(), request.OwnerId, request.Source, cancellationToken: cancellationToken);

            return new DeleteResponse(true, UpdateMessages.EntityDeletedSuccess, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete attachement");
            return new DeleteResponse(false, UpdateMessages.Error, null);
        }
    }
}
