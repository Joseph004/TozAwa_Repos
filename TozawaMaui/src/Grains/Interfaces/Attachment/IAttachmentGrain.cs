using Orleans;
using Grains;
using System.Threading.Tasks;
using Grains.Attachment.Models;

namespace Grains
{
    public interface IAttachmentGrain : IGrainWithGuidKey
    {
        Task SetAsync(AttachmentItem item);
        Task ActivateAsync(AttachmentItem item);
        Task ClearAsync();
        Task<AttachmentItem> GetAsync();
    }
}