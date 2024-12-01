﻿using Grains.Models.LoggedInState.Store;
using System.Collections.Immutable;

namespace Grains
{
    public class LoggedInManagerGrain([PersistentState("State")] IPersistentState<LoggedInStates> state) : Grain, ILoggedInManagerGrain
    {
        private readonly IPersistentState<LoggedInStates> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (_state.State.Items == null)
            {
                _state.State.Items = [];
            }

            await Task.CompletedTask;
        }
        
        public async Task RegisterAsync(Guid itemKey, LoggedInItem LoggedInItem)
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