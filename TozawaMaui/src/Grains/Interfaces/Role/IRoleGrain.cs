
namespace Grains
{
    public interface IRoleGrain : IGrainWithGuidKey
    {
        Task SetAsync(RoleItem item);
        Task ActivateAsync(RoleItem item);
        Task ClearAsync();
        Task<RoleItem> GetAsync();
    }
}