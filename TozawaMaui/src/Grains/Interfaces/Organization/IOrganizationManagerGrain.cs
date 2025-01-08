using System.Collections.Immutable;

namespace Grains
{
    public interface IOrganizationManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(Guid itemKey, OrganizationItem user);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey);

        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}