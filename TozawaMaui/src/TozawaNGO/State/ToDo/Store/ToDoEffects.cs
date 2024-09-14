using Fluxor;
using Grains;
using TozawaNGO.Services;
using Microsoft.AspNetCore.SignalR.Client;
using TozawaNGO.Models;
using ShareRazorClassLibrary.Helpers;

namespace TozawaNGO.State.ToDo.Store;

public class Effects(TodoService todoService)
{
    [EffectMethod(typeof(ToDoDataAction))]
    public async Task HandleToDoDataAction(IDispatcher dispatcher)
    {
        List<TodoNotification> notifications = [];
        var subscription = await todoService.SubscribeAsync(SystemTextId.ToDoOwnerId, notification => Task.Run(() =>
             HandleNotificationAsync(notifications, notification)));

        var todos = new TodoKeyedCollection();
        var data = await todoService.GetAllAsync(SystemTextId.ToDoOwnerId);

        foreach (var item in data)
        {
            todos.Add(item);
        }

        var hubConnection = await StartHubConnection();
        AddToDoDataListener(hubConnection, dispatcher);
        dispatcher.Dispatch(new ToDoDataFechedAction(todos, subscription, notifications, hubConnection));
    }

    private async Task<HubConnection> StartHubConnection()
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:8081/hubs/clienthub")
            .Build();

        if (hubConnection.State == HubConnectionState.Connected)
            await hubConnection.StopAsync();

        await hubConnection.StartAsync();
        if (hubConnection.State == HubConnectionState.Connected)
            Console.WriteLine("connection started");

        return hubConnection;
    }

    private void AddToDoDataListener(HubConnection hubConnection, IDispatcher dispatcher)
    {
        hubConnection.On<Guid>("ToDoAdded", (id) =>
        dispatcher.Dispatch(new LoadItemAction(id)));
    }

    [EffectMethod]
    public async Task OnLoadItem(LoadItemAction action, IDispatcher dispatcher)
    {
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