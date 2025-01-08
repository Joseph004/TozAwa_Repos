using System.Collections.Immutable;

namespace Grains
{
    public interface IMemberManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(Guid itemKey, MemberItem user);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey);

        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}