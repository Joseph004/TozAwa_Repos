using System.Collections.Immutable;

namespace Grains
{
    public interface ILoggedInManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(Guid itemKey, LoggedInItem user);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey);

        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}