using System.Collections.Immutable;
using Fluxor;

namespace TozawaNGO.State.SignalR.Store
{
    public class HubSetConnectedAction(bool connected)
    {
        public bool Connected { get; init; } = connected;
    }

    public class HubStartAction { }

    public class HubToDoChangedAction
    {
        public HubChangeType ChangeType { get; init; }
        public Guid? Id { get; init; }

        public HubToDoChangedAction(HubChangeType changeType, Guid id)
        {
            ChangeType = changeType;
            Id = id;
        }

        public HubToDoChangedAction(HubChangeType changeType)
        {
            ChangeType = changeType;
            Id = null;
        }
    }
}