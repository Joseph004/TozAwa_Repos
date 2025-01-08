using Grains.Auth.Models.Dtos.Backend;

namespace Grains
{
    [Immutable]
    public class AddressNotification(Guid itemKey, AddressItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public AddressItem Item { get; } = item;
    }
}