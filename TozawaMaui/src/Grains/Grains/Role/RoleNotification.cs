using Grains.Auth.Models.Dtos.Backend;

namespace Grains
{
    [Immutable]
    public class RoleNotification(Guid itemKey, RoleItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public RoleItem Item { get; } = item;
    }
}