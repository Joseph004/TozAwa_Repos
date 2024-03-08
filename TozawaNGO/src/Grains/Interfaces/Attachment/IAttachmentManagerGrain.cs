using Grains.Attachment.Models;
using Orleans;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public interface IAttachmentManagerGrain : IGrainWithGuidKey
    {
        [Alias("RegisterAsync")]
        Task RegisterAsync(Guid itemKey, AttachmentItem attachment);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey);

        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();
    }
}