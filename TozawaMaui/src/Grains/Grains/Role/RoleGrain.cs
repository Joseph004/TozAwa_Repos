using Orleans.Runtime;
using Grains.Models.Role.Store;

namespace Grains
{
    public class RoleGrain([PersistentState("State")] IPersistentState<RoleState> state) : Grain, IRoleGrain
    {
        private readonly IPersistentState<RoleState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<RoleItem> GetAsync() => Task.FromResult(_state.State.Role);

        public async Task ActivateAsync(RoleItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Role = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IRoleManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Id, item);
        }
        public async Task SetAsync(RoleItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Role = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IRoleManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Id, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<RoleNotification>(streamId)
                .OnNextAsync(new RoleNotification(item.Id, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Role == null) return;

            // hold on to the keys
            var itemKey = _state.State.Role.Id;
            var item = _state.State.Role;
            var ownerKey = _state.State.Role.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<IRoleManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<RoleNotification>(streamId)
                .OnNextAsync(new RoleNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}