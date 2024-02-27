using Fluxor;
using Grains;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.State.ToDo.Store;

[FeatureState]
public record ToDoState
{
    public string NewItem { get; init; }
    public bool IsLoading { get; init; }
    public bool Initialized { get; init; }
    public TodoKeyedCollection Todos { get; init; } = new();
    public Orleans.Streams.StreamSubscriptionHandle<TodoNotification> Subscription { get; init; }
}