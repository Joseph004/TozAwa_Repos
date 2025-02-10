using System.Collections.Immutable;

namespace Grains
{
    public interface IRoleManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(Guid itemKey, RoleItem user);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey);
        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}