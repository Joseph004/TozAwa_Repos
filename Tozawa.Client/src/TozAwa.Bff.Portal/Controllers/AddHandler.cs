using MediatR;
using Newtonsoft.Json;
using Tozawa.Bff.Portal.NotificationHandlers;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.Controllers
{
#nullable enable
    public abstract class AddHandler
    {
        protected readonly IMediator _mediator;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger _logger;

        public AddHandler(IMediator mediator, ICurrentUserService currentUserService, ILogger logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task HandleAdd(UpdateEntityType entityType, Models.Dtos.Backend.MemberDto member)
        {
            try
            {
                var notification = new EntityAddedNotification(entityType, _currentUserService.User.UserName, member, null);
                await _mediator.Publish(notification, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to catch entity. Error message {exm} request {req}", ex.Message, JsonConvert.SerializeObject(member));
            }
        }

        public async Task HandleAddMultiple(UpdateEntityType entityType, List<Models.Dtos.Backend.MemberDto> members)
        {
            try
            {
                var notification = new EntityAddedNotification(entityType, _currentUserService.User.UserName, null, members);
                await _mediator.Publish(notification, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to catch entities. Error message {exm} request {req}", ex.Message, JsonConvert.SerializeObject(members));
            }
        }

    }
}