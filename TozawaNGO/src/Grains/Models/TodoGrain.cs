using Orleans;
using Orleans.Runtime;
using Grains;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class TodoGrain([PersistentState("State")] IPersistentState<TodoGrain.State> state) : Grain, ITodoGrain
    {
        private readonly IPersistentState<State> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<TodoItem> GetAsync() => Task.FromResult(_state.State.Item);

        public async Task SetAsync(TodoItem item)
        {
            // ensure the key is consistent
            if (item.Key != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Item = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<ITodoManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Key);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<TodoNotification>(streamId)
                .OnNextAsync(new TodoNotification(item.Key, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Item == null) return;

            // hold on to the keys
            var itemKey = _state.State.Item.Key;
            var ownerKey = _state.State.Item.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<ITodoManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<TodoNotification>(streamId)
                .OnNextAsync(new TodoNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }

        public class State
        {
            public TodoItem Item { get; set; }
        }
    }
}