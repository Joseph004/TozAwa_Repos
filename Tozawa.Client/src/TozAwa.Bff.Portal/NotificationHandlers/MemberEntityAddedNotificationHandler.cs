using MediatR;
using Tozawa.Bff.Portal.Services;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.NotificationHandlers;

public class MemberEntityAddedNotificationHandler : INotificationHandler<EntityAddedNotification>
{
    private readonly IMemberService _memberService;

    public MemberEntityAddedNotificationHandler(IMemberService memberService)
    {
        _memberService = memberService;
    }
    public async Task Handle(EntityAddedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.EntityType != UpdateEntityType.Member)
        {
            await Task.CompletedTask;
            return;
        }
        var items = await _memberService.GetItems();
        if (notification.Entity != null && notification.Entity is Models.Dtos.Backend.MemberDto Member)
        {
            AddItemToCache(items, Member);
        }
        if (notification.Entities != null && notification.Entities is IEnumerable<Models.Dtos.Backend.MemberDto> addedItems)
        {
            foreach (var item in addedItems)
            {
                AddItemToCache(items, item);
            }
        }
        await _memberService.SortItems();
    }

    private static void AddItemToCache(List<Models.Dtos.Backend.MemberDto> items, Models.Dtos.Backend.MemberDto item)
    {
        var existing = items.FirstOrDefault(x => x.Code == item.Code);
        if (existing != null)
        {
            items.Remove(existing);
        }
        items.Add(item);

    }
}
