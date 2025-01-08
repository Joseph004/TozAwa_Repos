using System.Collections.Immutable;

namespace Grains
{
    public interface IFeatureManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(Guid itemKey, FeatureItem user);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey);
        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}