using System.Collections.Immutable;

namespace Grains
{
    public interface IEmailGuidManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(string itemKey, EmailGuidItem user);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(string itemKey);
        [Alias("GetAllAsync")]
        Task<ImmutableArray<string>> GetAllAsync();
    }
}