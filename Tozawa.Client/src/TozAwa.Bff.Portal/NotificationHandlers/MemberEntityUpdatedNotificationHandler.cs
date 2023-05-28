

using MediatR;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.NotificationHandlers;

public class MemberEntityUpdateNotificationHandler : EntityUpdateService, INotificationHandler<EntityUpdatedNotification>
{
    private readonly IMemberService _memberService;

    public MemberEntityUpdateNotificationHandler(IMemberService memberService)
    {
        _memberService = memberService;
    }
    public async Task Handle(EntityUpdatedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.EntityType == UpdateEntityType.Member)
        {
            var items = await _memberService.GetItems();
            var updatedItem = items.FirstOrDefault(x => x.Id == notification.Id);
            if (updatedItem == null) return;

            _memberService.ClearCache();
            Update(updatedItem, notification.UpdatedProperties, notification.Username);
        }
    }
}