
namespace Grains
{
    public interface IOrganizationGrain : IGrainWithGuidKey
    {
        Task SetAsync(OrganizationItem item);
        Task ActivateAsync(OrganizationItem item);
        Task ClearAsync();
        Task<OrganizationItem> GetAsync();
    }
}