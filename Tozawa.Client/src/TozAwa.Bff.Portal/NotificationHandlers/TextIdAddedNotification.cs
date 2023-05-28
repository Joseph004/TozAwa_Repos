

using MediatR;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.NotificationHandlers;

public class TextIdAddedNotification : INotification
{
#nullable enable
    public Guid Id { get; set; }
    public UpdateEntityType EntityType { get; }
    public Guid? NameTextId { get; set; }
    public Guid? DescriptionTextId { get; set; }
    public TextIdAddedNotification(UpdateEntityType entityType, Guid id)
    {
        EntityType = entityType;
        Id = id;
    }
}
