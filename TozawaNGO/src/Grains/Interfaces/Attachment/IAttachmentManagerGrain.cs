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
        Task RegisterAsync(Guid itemKey, Guid ownerKey);
        [Alias("UnregisterAsync")]
        Task UnregisterAsync(Guid itemKey, Guid ownerKey);

        [Alias("GetAllAsync")]
        Task<ImmutableArray<Guid>> GetAllAsync();

        [Alias("GetAllByOwnerIdAsync")]
        Task<ImmutableArray<Guid>> GetAllByOwnerIdAsync(Guid ownerId);
    }
}