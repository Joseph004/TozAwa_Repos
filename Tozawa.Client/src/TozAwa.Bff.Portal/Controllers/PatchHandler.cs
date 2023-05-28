

using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Tozawa.Bff.Portal.ClientMessages;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.NotificationHandlers;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Controllers;

public abstract class PatchHandler
{
    protected readonly ILogger _logger;
    protected readonly ICurrentUserService _currentUserService;
    protected readonly IMediator _mediator;

    public PatchHandler(ILogger logger, ICurrentUserService currentUserService, IMediator mediator)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task HandlePatch(JsonPatchDocument request, Guid entityId, UpdateEntityType entityType, CancellationToken cancellationToken)
    {
        try
        {
            var updateNotification = new EntityUpdatedNotification(entityId, entityType, _currentUserService.User.UserName);
            foreach (var operation in request.Operations)
            {
                updateNotification.AddUpdatedProperty(operation.path, operation.value);
            }

            await _mediator.Publish(updateNotification, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity. Exception message {ex.Message} request {req}", ex.Message, JsonConvert.SerializeObject(request));
        }
    }
}
