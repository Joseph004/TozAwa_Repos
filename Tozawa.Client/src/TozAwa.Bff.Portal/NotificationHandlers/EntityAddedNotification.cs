

using MediatR;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.NotificationHandlers;

public class EntityAddedNotification : INotification
{
    #nullable enable
    public UpdateEntityType EntityType { get; }
    public string Username { get; }
    public object? Entity { get; }
    public IEnumerable<object>? Entities { get; }
    public EntityAddedNotification(UpdateEntityType entityType, string username, object? entity, IEnumerable<object>? entities)
    {
        EntityType = entityType;
        Username = username;
        Entity = entity;
        Entities = entities;
    }
}
