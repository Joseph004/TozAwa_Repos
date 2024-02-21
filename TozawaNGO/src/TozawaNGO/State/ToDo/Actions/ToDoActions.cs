using Grains;
using Orleans.Streams;
using TozawaNGO.Models.Dtos;

namespace TozawaNGO.State.ToDo.Actions;
public record HandleInputTextToDoAction(string newItem)
{
    public string NewItem { get; } = newItem;
}
public record ToDoDataFechedAction(TodoKeyedCollection todos, StreamSubscriptionHandle<TodoNotification> subscription, List<TodoNotification> notifications);
public record ToDoDataAction;
public record UnSubscribeAction;
public record ToDoAddAction(string newItem)
{
    public string NewItem { get; } = newItem;
}
public record ToDoAddAfterAction(TodoItem todo);