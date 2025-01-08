
namespace Grains
{
    public interface IFeatureGrain : IGrainWithGuidKey
    {
        Task SetAsync(FeatureItem item);
        Task ActivateAsync(FeatureItem item);
        Task ClearAsync();
        Task<FeatureItem> GetAsync();
    }
}