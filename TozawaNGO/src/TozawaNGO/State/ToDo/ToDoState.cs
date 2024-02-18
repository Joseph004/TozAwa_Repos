using Fluxor;

namespace TozawaNGO.State.ToDo;

[FeatureState]
public record ToDoState
{
    public string NewItem { get; init; }
}