using Orleans.Runtime;
using Microsoft.AspNetCore.SignalR;
using Grains.Models.ToDo.Store;
using Shared.SignalR;

namespace Grains
{
    public class TodoGrain([PersistentState("State")] IPersistentState<ToDoState> state, IHubContext<ClientHub> hubContext) : Grain, ITodoGrain
    {
        private IHubContext<ClientHub> _hubContext = hubContext;
        private readonly IPersistentState<ToDoState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<TodoItem> GetAsync() => Task.FromResult(_state.State.ToDo);

        public async Task SetAsync(TodoItem item)
        {
             // ensure the key is consistent
            if (item.Key != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.ToDo = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<ITodoManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.Key, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<TodoNotification>(streamId)
                .OnNextAsync(new TodoNotification(item.Key, item))
                .Ignore();

            await NotifyHub("ToDoAdded", item.Key);
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.ToDo == null) return;

            // hold on to the keys
            var itemKey = _state.State.ToDo.Key;
            var item = _state.State.ToDo;
            var ownerKey = _state.State.ToDo.OwnerKey;

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

            await NotifyHub("ToDoDeleted", itemKey);

            // no need to stay alive anymore
            DeactivateOnIdle();
        }

        private async Task NotifyHub(string method, Guid id)
        {
            await _hubContext.Clients.All.SendAsync(method, id);
        }

        private async Task NotifyHub(string method)
        {
            await _hubContext.Clients.All.SendAsync(method);
        }
    }
}