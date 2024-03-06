using Grains.Models.ToDo.Store;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    public class TodoManagerGrain([PersistentState("State")] IPersistentState<ToDoStates> state) : Grain, ITodoManagerGrain
    {
        private readonly IPersistentState<ToDoStates> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var items = new List<TodoItem>();
            if (_state.State.Items == null)
            {
                _state.State.Items = [];
            }

            foreach (var item in items)
            {
                _state.State.Items.Add(item.Key);
            }

            return base.OnActivateAsync(cancellationToken);
        }

        public async Task RegisterAsync(Guid itemKey, TodoItem todoItem)
        {
            _state.State.Items.Add(itemKey);
            await _state.WriteStateAsync();
        }

        public async Task UnregisterAsync(Guid itemKey)
        {
            _state.State.Items.Remove(itemKey);
            await _state.WriteStateAsync();
        }

        public Task<ImmutableArray<Guid>> GetAllAsync() =>
            Task.FromResult(ImmutableArray.CreateRange(_state.State.Items));
    }
}