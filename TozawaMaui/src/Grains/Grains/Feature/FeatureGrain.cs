using Grains.Models.Feature.Store;

namespace Grains
{
    public class FeatureGrain([PersistentState("State")] IPersistentState<FeatureState> state) : Grain, IFeatureGrain
    {
        private readonly IPersistentState<FeatureState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<FeatureItem> GetAsync() => Task.FromResult(_state.State.Feature);

        public async Task ActivateAsync(FeatureItem item)
        {
            // ensure the key is consistent
            if (item.TextId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Feature = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IFeatureManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.TextId, item);
        }
        public async Task SetAsync(FeatureItem item)
        {
            // ensure the key is consistent
            if (item.TextId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Feature = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IFeatureManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.TextId, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<FeatureNotification>(streamId)
                .OnNextAsync(new FeatureNotification(item.TextId, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Feature == null) return;

            // hold on to the keys
            var itemKey = _state.State.Feature.TextId;
            var item = _state.State.Feature;
            var ownerKey = _state.State.Feature.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<IFeatureManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<FeatureNotification>(streamId)
                .OnNextAsync(new FeatureNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}