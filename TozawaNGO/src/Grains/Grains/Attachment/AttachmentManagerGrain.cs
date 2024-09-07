using Grains.Models.Attachment.Store;
using Orleans.Runtime;
using System.Collections.Immutable;

namespace Grains
{
    public class AttachmentManagerGrain([PersistentState("State")] IPersistentState<AttachmentStates> state) : Grain, IAttachmentManagerGrain
    {
        private readonly IPersistentState<AttachmentStates> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (_state.State.Items == null)
            {
                _state.State.Items = [];
            }

            await Task.CompletedTask;
        }

        public async Task RegisterAsync(Guid itemKey, Guid ownerKey)
        {
            _state.State.Items.Add(itemKey + "|" + ownerKey);
            await _state.WriteStateAsync();
        }

        public async Task UnregisterAsync(Guid itemKey, Guid ownerKey)
        {
            _state.State.Items.Remove(itemKey + "|" + ownerKey);
            await _state.WriteStateAsync();
        }

        public Task<ImmutableArray<Guid>> GetAllAsync()
        {
            if (_state.State.Items.Count == 0) return Task.FromResult(ImmutableArray.CreateRange(Array.Empty<Guid>()));
            var guids = _state.State.Items.Select(x => Guid.Parse(x.Split("|")[0]));
            return Task.FromResult(ImmutableArray.CreateRange(guids));
        }

        public Task<ImmutableArray<Guid>> GetAllByOwnerIdAsync(Guid ownerId)
        {
            if (_state.State.Items.Count == 0) return Task.FromResult(ImmutableArray.CreateRange(Array.Empty<Guid>()));
            var guids = _state.State.Items.Where(y => Guid.Parse(y.Split("|")[1]) == ownerId).Select(x => Guid.Parse(x.Split("|")[0]));
            return Task.FromResult(ImmutableArray.CreateRange(guids));
        }
    }
}