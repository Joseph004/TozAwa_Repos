
using MediatR;
using Tozawa.Bff.Portal.Controllers;
using Tozawa.Bff.Portal.HttpClients;
using Tozawa.Bff.Portal.Models.Request.Backend;
using Tozawa.Bff.Portal.Models.ResponseRequests;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.NotificationHandlers;

public class MemberTextIdAddedNotificationHandler : INotificationHandler<TextIdAddedNotification>
{
    protected readonly ITozAwaAuthHttpClient _httpClient;
    protected readonly ICurrentUserService _currentUserService;
    private readonly IMemberService _service;
    private readonly IMemberConverter _converter;

    public MemberTextIdAddedNotificationHandler(ITozAwaAuthHttpClient httpClient, IMemberService service, IMemberConverter converter,
            ICurrentUserService currentUserService)
    {
        _httpClient = httpClient;
        _currentUserService = currentUserService;
        _service = service;
        _converter = converter;
    }
    public async Task Handle(TextIdAddedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.EntityType != UpdateEntityType.Member)
        {
            await Task.CompletedTask;
            return;
        }

        if (notification.DescriptionTextId.HasValue && !Guid.Empty.Equals(notification.DescriptionTextId))
        {
            var uri = $"member/{notification.Id}";
            var request = new Models.Request.Frontend.PatchMemberRequest
            {
                DescriptionTextId = notification.DescriptionTextId
            };
            var entity = await _httpClient.Patch<UpdateResponse>(uri, new PatchMemberRequest(request).ToPatchDocument());
        }

        var updatedEntity = await _service.GetItem(notification.Id);
    }
}