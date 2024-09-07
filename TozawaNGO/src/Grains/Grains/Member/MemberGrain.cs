using Orleans.Runtime;
using Grains.Models.Member.Store;

namespace Grains
{
    public class MemberGrain([PersistentState("State")] IPersistentState<MemberState> state) : Grain, IMemberGrain
    {
        private readonly IPersistentState<MemberState> _state = state;

        private Guid GrainKey => this.GetPrimaryKey();

        public Task<MemberItem> GetAsync() => Task.FromResult(_state.State.Member);

        public async Task ActivateAsync(MemberItem item)
        {
            // ensure the key is consistent
            if (item.UserId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Member = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IMemberManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.UserId, item);
        }
        public async Task SetAsync(MemberItem item)
        {
            // ensure the key is consistent
            if (item.UserId != GrainKey)
            {
                throw new InvalidOperationException();
            }

            // save the item
            _state.State.Member = item;
            await _state.WriteStateAsync();

            // register the item with its owner list
            await GrainFactory.GetGrain<IMemberManagerGrain>(item.OwnerKey)
                .RegisterAsync(item.UserId, item);

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), item.OwnerKey);
            this.GetStreamProvider("SMS").GetStream<MemberNotification>(streamId)
                .OnNextAsync(new MemberNotification(item.UserId, item))
                .Ignore();
        }

        public async Task ClearAsync()
        {
            // fast path for already cleared state
            if (_state.State.Member == null) return;

            // hold on to the keys
            var itemKey = _state.State.Member.UserId;
            var item = _state.State.Member;
            var ownerKey = _state.State.Member.OwnerKey;

            // unregister from the registry
            await GrainFactory.GetGrain<IMemberManagerGrain>(ownerKey)
                .UnregisterAsync(itemKey);

            // clear the state
            await _state.ClearStateAsync();

            // notify listeners - best effort only
            var streamId = StreamId.Create(nameof(Grains), ownerKey);
            this.GetStreamProvider("SMS").GetStream<MemberNotification>(streamId)
                .OnNextAsync(new MemberNotification(itemKey, null))
                .Ignore();

            // no need to stay alive anymore
            DeactivateOnIdle();
        }
    }
}