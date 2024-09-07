using Orleans.Runtime;
using Grains.Models.Translation.Store;

namespace Grains
{
    public class TranslationGrain([PersistentState("State")] IPersistentState<TranslationState> state) : Grain, ITranslationGrain
    {
        private readonly IPersistentState<TranslationState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<TranslationItem> GetAsync() => Task.FromResult(_state.State.Translation);

        public async Task ActivateAsync(TranslationItem item)
        {
            // ensure the key is consistent
            if (item.TextId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Translation = item;
            await _state.WriteStateAsync();
        }
        public async Task SetAsync(TranslationItem item)
        {
            // ensure the key is consistent
            if (item.TextId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Translation = item;
            await _state.WriteStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerId);
            this.GetStreamProvider("SMS").GetStream<TranslationNotification>(streamId)
                .OnNextAsync(new TranslationNotification(item.TextId, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Translation == null) return;

            // hold on to the keys
            var itemKey = _state.State.Translation.TextId;
            var item = _state.State.Translation;
            var ownerKey = _state.State.Translation.OwnerId;

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<TranslationNotification>(streamId)
                .OnNextAsync(new TranslationNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}