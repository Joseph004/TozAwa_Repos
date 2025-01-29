using Orleans.Runtime;
using Grains.Models.EmailGuid.Store;

namespace Grains
{
    public class EmailGuidGrain([PersistentState("State")] IPersistentState<EmailGuidState> state) : Grain, IEmailGuidGrain
    {
        private readonly IPersistentState<EmailGuidState> _state = state;

        private string GrainKey => this.GetPrimaryKeyString();

        public Task<EmailGuidItem> GetAsync() => Task.FromResult(_state.State.EmailGuid);

        public async Task ActivateAsync(EmailGuidItem item)
        {
            // ensure the key is consistent
            if (item.Email != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.EmailGuid = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IEmailGuidManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Email, item);
        }
        public async Task SetAsync(EmailGuidItem item)
        {
            // ensure the key is consistent
            if (item.Email != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.EmailGuid = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IEmailGuidManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Email, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<EmailGuidNotification>(streamId)
                .OnNextAsync(new EmailGuidNotification(item.Email, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.EmailGuid == null) return;

            // hold on to the keys
            var itemKey = _state.State.EmailGuid.Email;
            var item = _state.State.EmailGuid;
            var ownerKey = _state.State.EmailGuid.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<IEmailGuidManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<EmailGuidNotification>(streamId)
                .OnNextAsync(new EmailGuidNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}