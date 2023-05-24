

using MediatR;
using Tozawa.Client.Portal.Models.Enums;

namespace Tozawa.Bff.Portal.NotificationHandlers;

public class EntityUpdatedNotification : INotification
{
#nullable enable
    public Guid Id { get; }
    public UpdateEntityType EntityType { get; }
    public object? PatchedEntity { get; set; }
    public string Username { get; }

    public EntityUpdatedNotification(Guid id, UpdateEntityType entityType, string username)
    {
        Id = id;
        EntityType = entityType;
        UpdatedProperties = new System.Dynamic.ExpandoObject();
        PatchedEntity = null;
        Username = username;
    }
#nullable disable warnings
    public void AddUpdatedProperty(string key, object? obj)
    {
        if (UpdatedProperties is IDictionary<string, object> updates)
        {
            updates[key] = obj;
        }
    }
#nullable enable warnings

    public dynamic UpdatedProperties { get; }
}
