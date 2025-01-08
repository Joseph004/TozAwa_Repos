using Grains.Models.Feature.Store;
using System.Collections.Immutable;

namespace Grains
{
    public class FeatureManagerGrain([PersistentState("State")] IPersistentState<FeatureStates> state) : Grain, IFeatureManagerGrain
    {
        private readonly IPersistentState<FeatureStates> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (_state.State.Items == null)
            {
                _state.State.Items = [];
            }

            await Task.CompletedTask;
        }
        
        public async Task RegisterAsync(Guid itemKey, FeatureItem FeatureItem)
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