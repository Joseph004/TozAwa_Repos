using System.Collections.Immutable;
using Fluxor;

namespace TozawaNGO.State.SignalR.Store
{
    [FeatureState]
    public record HubState
    {
        public bool Connected { get; init; } = false;
    }
}