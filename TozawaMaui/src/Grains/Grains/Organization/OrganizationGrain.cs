using Orleans.Runtime;
using Grains.Models.Organization.Store;

namespace Grains
{
    public class OrganizationGrain([PersistentState("State")] IPersistentState<OrganizationState> state) : Grain, IOrganizationGrain
    {
        private readonly IPersistentState<OrganizationState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<OrganizationItem> GetAsync() => Task.FromResult(_state.State.Organization);

        public async Task ActivateAsync(OrganizationItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Organization = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IOrganizationManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Id, item);
        }
        public async Task SetAsync(OrganizationItem item)
        {
            // ensure the key is consistent
            if (item.Id != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Organization = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IOrganizationManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Id, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<OrganizationNotification>(streamId)
                .OnNextAsync(new OrganizationNotification(item.Id, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Organization == null) return;

            // hold on to the keys
            var itemKey = _state.State.Organization.Id;
            var item = _state.State.Organization;
            var ownerKey = _state.State.Organization.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<IOrganizationManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<OrganizationNotification>(streamId)
                .OnNextAsync(new OrganizationNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}