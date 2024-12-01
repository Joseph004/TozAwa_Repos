using Grains.Models.LoggedInState.Store;

namespace Grains
{
    public class LoggedInGrain([PersistentState("State")] IPersistentState<LoggedInState> state) : Grain, ILoggedInGrain
    {
        private readonly IPersistentState<LoggedInState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<LoggedInItem> GetAsync() => Task.FromResult(_state.State.LoggedIn);

        public async Task ActivateAsync(LoggedInItem item)
        {
            // ensure the key is consistent
            if (item.UserId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.LoggedIn = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<ILoggedInManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.UserId, item);
        }
        public async Task SetAsync(LoggedInItem item)
        {
            // ensure the key is consistent
            if (item.UserId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.LoggedIn = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<ILoggedInManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.UserId, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<LoggedInNotification>(streamId)
                .OnNextAsync(new LoggedInNotification(item.UserId, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.LoggedIn == null) return;

            // hold on to the keys
            var itemKey = _state.State.LoggedIn.UserId;
            var item = _state.State.LoggedIn;
            var ownerKey = _state.State.LoggedIn.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<ILoggedInManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<LoggedInNotification>(streamId)
                .OnNextAsync(new LoggedInNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}