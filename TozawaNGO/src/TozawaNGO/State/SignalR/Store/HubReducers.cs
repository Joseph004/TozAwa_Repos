using System.Collections.Immutable;
using Fluxor;

namespace TozawaNGO.State.SignalR.Store
{
    public static class HubReducers
    {
        [ReducerMethod]
        public static HubState OnSetConnected(HubState state, HubSetConnectedAction action)
        {
            return state with
            {
                Connected = action.Connected
            };
        }
    }
}
