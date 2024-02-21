using Fluxor;
using Grains;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.State.ToDo;

[FeatureState]
public record ToDoState
{
    public string NewItem { get; init; }
    public bool IsLoading { get; init; }
    public TodoKeyedCollection Todos { get; init; } = new();
    public Orleans.Streams.StreamSubscriptionHandle<TodoNotification> Subscription { get; init; }
}