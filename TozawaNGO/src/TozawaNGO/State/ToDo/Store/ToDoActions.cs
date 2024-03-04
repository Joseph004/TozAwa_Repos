using Grains;
using Microsoft.AspNetCore.SignalR.Client;
using Orleans.Streams;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.State.ToDo.Store;
public record HandleInputTextToDoAction(string newItem)
{
    public string NewItem { get; } = newItem;
}
public record ToDoDataFechedAction(TodoKeyedCollection todos, StreamSubscriptionHandle<TodoNotification> subscription, List<TodoNotification> notifications, HubConnection hubConnection);
public record ToDoDataAction;
public record UnSubscribeAction;
public record ToDoAddAction(string newItem)
{
    public string NewItem { get; } = newItem;
}
public record ScrollTopAction(double scrollTop)
{
    public double ScrollTop { get; } = scrollTop;
}
public record ToDoAddAfterAction(TodoItem todo);
public record IncrementToDoAction;
public record DecrementToDoAction;
public class LoadItemAction(Guid id)
{
    public Guid Id { get; set; } = id;
}
public class RemoveItemAction(Guid id)
{
    public Guid Id { get; set; } = id;
}
public class LoadDataAction { }
