using Fluxor;
using Grains;
using Microsoft.AspNetCore.SignalR;
using TozawaNGO.Helpers;
using TozawaNGO.Services;
using Shared.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace TozawaNGO.State.ToDo.Store;

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

    [EffectMethod(typeof(LoadDataAction))]
    public async Task LoadData(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new ToDoDataAction());
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

    [EffectMethod(typeof(StartHubConnectionAction))]
    private async Task StartHubConnection(IDispatcher dispatcher)
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:8081/hubs/clienthub")
            .Build();
        await hubConnection.StartAsync();
        if (hubConnection.State == HubConnectionState.Connected)
            Console.WriteLine("connection started");

        dispatcher.Dispatch(new HubConnectionAfterAction(hubConnection));
    }

    [EffectMethod]
    public async Task OnLoadItem(LoadItemAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new ToDoDataAction());
        var todo = await todoService.GetAsync(action.Id);

        dispatcher.Dispatch(new ToDoAddAfterAction(todo));
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