using System.Collections.Immutable;

namespace Grains
{
    public interface IAddressManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(Guid itemKey, AddressItem user);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey);
        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}