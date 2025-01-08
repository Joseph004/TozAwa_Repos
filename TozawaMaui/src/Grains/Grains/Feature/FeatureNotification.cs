using Grains.Auth.Models.Dtos.Backend;

namespace Grains
{
    [Immutable]
    public class FeatureNotification(Guid itemKey, FeatureItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public FeatureItem Item { get; } = item;
    }
}