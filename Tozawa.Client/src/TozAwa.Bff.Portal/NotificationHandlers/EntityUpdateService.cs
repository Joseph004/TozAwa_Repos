

namespace Tozawa.Bff.Portal.NotificationHandlers;

public abstract class EntityUpdateService
{
    protected void Update(object entity, dynamic updatedProperties, string username)
    {
        var updates = (IDictionary<string, object>)updatedProperties;
        if (updates == null || !updates.Any()) return;
        var entityType = entity.GetType();

        foreach (var update in updates)
        {
            var updatedProperty = entityType.GetProperty(update.Key);
            if (updatedProperty == null)
            {
                continue;
            }
            updatedProperty.SetValue(entity, update.Value);
        }

        var modifiedDate = entityType.GetProperty("ModifiedDate");
        if (modifiedDate != null)
        {
            modifiedDate.SetValue(entity, DateTime.UtcNow);
        }

        var modifiedBy = entityType.GetProperty("ModifiedBy");
        if (modifiedBy != null)
        {
            modifiedBy.SetValue(entity, username);
        }

    }
}