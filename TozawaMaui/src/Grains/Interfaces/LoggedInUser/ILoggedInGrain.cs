
namespace Grains
{
    public interface ILoggedInGrain : IGrainWithGuidKey
    {
        Task SetAsync(LoggedInItem item);
        Task ActivateAsync(LoggedInItem item);
        Task ClearAsync();
        Task<LoggedInItem> GetAsync();
    }
}