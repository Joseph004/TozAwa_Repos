using Orleans.Runtime;
using Grains.Models.Address.Store;

namespace Grains
{
    public class AddressGrain([PersistentState("State")] IPersistentState<AddressState> state) : Grain, IAddressGrain
    {
        private readonly IPersistentState<AddressState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<AddressItem> GetAsync() => Task.FromResult(_state.State.Address);

        public async Task ActivateAsync(AddressItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Address = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IAddressManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Id, item);
        }
        public async Task SetAsync(AddressItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Address = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IAddressManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Id, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<AddressNotification>(streamId)
                .OnNextAsync(new AddressNotification(item.Id, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Address == null) return;

            // hold on to the keys
            var itemKey = _state.State.Address.Id;
            var item = _state.State.Address;
            var ownerKey = _state.State.Address.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<IAddressManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<AddressNotification>(streamId)
                .OnNextAsync(new AddressNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}