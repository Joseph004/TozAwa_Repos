using Orleans;
using Grains;
using System.Threading.Tasks;
using Grains.Auth.Models.Authentication;

namespace Grains
{
    public interface IMemberGrain : IGrainWithGuidKey
    {
        Task SetAsync(MemberItem item);
        Task ActivateAsync(MemberItem item);
        Task ClearAsync();
        Task<MemberItem> GetAsync();
    }
}