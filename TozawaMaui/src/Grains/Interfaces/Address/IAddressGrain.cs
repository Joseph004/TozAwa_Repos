
namespace Grains
{
    public interface IAddressGrain : IGrainWithGuidKey
    {
        Task SetAsync(AddressItem item);
        Task ActivateAsync(AddressItem item);
        Task ClearAsync();
        Task<AddressItem> GetAsync();
    }
}