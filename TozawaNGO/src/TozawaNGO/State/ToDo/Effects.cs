using Fluxor;
using Grains;
using TozawaNGO.Helpers;
using TozawaNGO.Services;
using TozawaNGO.State.ToDo.Actions;

namespace TozawaNGO.State.ToDo;

public class Effects(TodoService todoService)
{
    [EffectMethod(typeof(ToDoDataAction))]
    public async Task HandleToDoDataAction(IDispatcher dispatcher)
    {
        List<TodoNotification> notifications = [];
        var subscription = await todoService.SubscribeAsync(SystemTextId.ToDoOwnerId, notification => Task.Run(() =>
             HandleNotificationAsync(notifications, notification)));

        var todos = new Models.Dtos.TodoKeyedCollection();

        foreach (var item in await todoService.GetAllAsync(SystemTextId.ToDoOwnerId))
        {
            todos.Add(item);
        }

        dispatcher.Dispatch(new ToDoDataFechedAction(todos, subscription, notifications));
    }

    [EffectMethod]
    public async Task HandleToDoAddAction(ToDoAddAction action, IDispatcher dispatcher)
    {
        var todo = new TodoItem(Guid.NewGuid(), action.NewItem, false, SystemTextId.ToDoOwnerId);
        await todoService.SetAsync(todo);
        dispatcher.Dispatch(new ToDoAddAfterAction(todo));
    }

    private Task HandleNotificationAsync(List<TodoNotification> notifications, TodoNotification notification)
    {
        if (!notifications.Any(x => x.ItemKey == notification.ItemKey))
        {
            notifications.Add(notification);
        }
        return Task.CompletedTask;
    }
}