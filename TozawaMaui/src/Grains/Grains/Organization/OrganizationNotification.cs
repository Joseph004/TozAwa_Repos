using Grains.Auth.Models.Dtos.Backend;

namespace Grains
{
    [Immutable]
    public class OrganizationNotification(Guid itemKey, OrganizationItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public OrganizationItem Item { get; } = item;
    }
}