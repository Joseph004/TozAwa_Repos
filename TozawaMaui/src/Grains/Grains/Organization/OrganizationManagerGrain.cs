using Grains.Models.Organization.Store;
using System.Collections.Immutable;

namespace Grains
{
    public class OrganizationManagerGrain([PersistentState("State")] IPersistentState<OrganizationStates> state) : Grain, IOrganizationManagerGrain
    {
        private readonly IPersistentState<OrganizationStates> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (_state.State.Items == null)
            {
                _state.State.Items = [];
            }

            await Task.CompletedTask;
        }
        
        public async Task RegisterAsync(Guid itemKey, OrganizationItem OrganizationItem)
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