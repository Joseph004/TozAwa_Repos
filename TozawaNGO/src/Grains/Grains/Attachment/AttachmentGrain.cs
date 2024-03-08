using Orleans.Runtime;
using Grains.Models.Attachment.Store;

namespace Grains
{
    public class AttachmentGrain([PersistentState("State")] IPersistentState<AttachmentState> state) : Grain, IAttachmentGrain
    {
        private readonly IPersistentState<AttachmentState> _state = state;
        private Guid GrainKey => this.GetPrimaryKey();
        public Task<AttachmentItem> GetAsync() => Task.FromResult(_state.State.Attachment);

        public async Task ActivateAsync(AttachmentItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Attachment = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IAttachmentManagerGrain>(item.OwnerId)
                .RegisterAsync(item.Id, item);
        }
        public async Task SetAsync(AttachmentItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Attachment = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IAttachmentManagerGrain>(item.OwnerId)
                .RegisterAsync(item.Id, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerId);
            this.GetStreamProvider("SMS").GetStream<AttachmentNotification>(streamId)
                .OnNextAsync(new AttachmentNotification(item.Id, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Attachment == null) return;

            // hold on to the keys
            var itemKey = _state.State.Attachment.Id;
            var item = _state.State.Attachment;
            var ownerKey = _state.State.Attachment.OwnerId;

            // unregister from the registry
            await GrainFactory.GetGrain<IAttachmentManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<AttachmentNotification>(streamId)
                .OnNextAsync(new AttachmentNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}