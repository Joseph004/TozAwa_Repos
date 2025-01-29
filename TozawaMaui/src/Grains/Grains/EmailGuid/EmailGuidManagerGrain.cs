using Grains.Models.EmailGuid.Store;
using System.Collections.Immutable;

namespace Grains
{
    public class EmailGuidManagerGrain([PersistentState("State")] IPersistentState<EmailGuidStates> state) : Grain, IEmailGuidManagerGrain
    {
        private readonly IPersistentState<EmailGuidStates> _state = state;

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (_state.State.Items == null)
            {
                _state.State.Items = [];
            }

            await Task.CompletedTask;
        }
        
        public async Task RegisterAsync(string itemKey, EmailGuidItem EmailGuidItem)
        {
            _state.State.Items.Add(itemKey);
            await _state.WriteStateAsync();
        }

        public async Task UnregisterAsync(string itemKey)
        {
            _state.State.Items.Remove(itemKey);
            await _state.WriteStateAsync();
        }

        public Task<ImmutableArray<string>> GetAllAsync() =>
            Task.FromResult(ImmutableArray.CreateRange(_state.State.Items));
    }
}