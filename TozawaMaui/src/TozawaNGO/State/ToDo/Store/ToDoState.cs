using Fluxor;
using Grains;
using Microsoft.AspNetCore.SignalR.Client;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.State.ToDo.Store;

[FeatureState]
public record ToDoState
{
    public string NewItem { get; init; }
    public double ScrollTop { get; init; }
    public bool IsLoading { get; init; }
    public bool Initialized { get; init; }
    public HubConnection HubConnection { get; init; }
    public TodoKeyedCollection Todos { get; init; } = [];
    public Orleans.Streams.StreamSubscriptionHandle<TodoNotification> Subscription { get; init; }
}