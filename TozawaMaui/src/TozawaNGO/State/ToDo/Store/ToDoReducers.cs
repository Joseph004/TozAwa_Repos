using Fluxor;
using Grains;
using TozawaNGO.State.Counter.Actions;

namespace TozawaNGO.State.ToDo.Store;

public static class Redures
{
    [ReducerMethod]
    public static ToDoState ReduceHandleInputTextToDoAction(ToDoState state, HandleInputTextToDoAction action)
    {
        return new() { IsLoading = false, Subscription = state.Subscription, Todos = state.Todos, NewItem = action.NewItem, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop };
    }
    [ReducerMethod(typeof(UnSubscribeAction))]
    public static ToDoState ReduceUnSubscribeAction(ToDoState state)
    {
        state.Subscription?.UnsubscribeAsync();
        state.HubConnection.DisposeAsync();
        return new() { IsLoading = false, Subscription = state.Subscription, Todos = state.Todos, NewItem = state.NewItem, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop };
    }

    [ReducerMethod]
    public static ToDoState ReduceFetchDataAction(ToDoState state, ToDoDataAction action) => new() { IsLoading = true, NewItem = state.NewItem, ScrollTop = action.scrollTop };

    [ReducerMethod]
    public static ToDoState ReduceDataFetchedAction(ToDoState state, ToDoDataFechedAction action)
    {
        HandleNotificationAsync(action.notifications, state);
        return new() { IsLoading = false, Subscription = action.subscription, Todos = action.todos, NewItem = state.NewItem, HubConnection = action.hubConnection, ScrollTop = state.ScrollTop };
    }

    [ReducerMethod]
    public static ToDoState ReduceScrollTopAction(ToDoState state, ScrollTopAction action)
    {
        return new() { IsLoading = false, Subscription = state.Subscription, Todos = state.Todos, NewItem = state.NewItem, HubConnection = state.HubConnection, ScrollTop = action.scrollTop };
    }

    [ReducerMethod]
    public static ToDoState ReduceToDoAddAction(ToDoState state, ToDoAddAfterAction action)
    {
        if (state.Todos.TryGetValue(action.todo.Key, out var current))
        {
            if (action.todo.Timestamp > current.Timestamp)
            {
                state.Todos[state.Todos.IndexOf(current)] = action.todo;
            }
        }
        else
        {
            state.Todos.Add(action.todo);
        }

        return new() { IsLoading = false, Subscription = state.Subscription, Todos = state.Todos, NewItem = null, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop };
    }
    private static Task HandleNotificationAsync(List<TodoNotification> notifications, ToDoState state)
    {
        foreach (var notification in notifications)
        {
            if (notification.Item == null)
            {
                if (state.Todos.Remove(notification.ItemKey))
                {
                }
                return Task.CompletedTask;
            }
            if (state.Todos.TryGetValue(notification.Item.Key, out var current))
            {
                if (notification.Item.Timestamp > current.Timestamp)
                {
                    state.Todos[state.Todos.IndexOf(current)] = notification.Item;
                }
                return Task.CompletedTask;
            }
            state.Todos.Add(notification.Item);
        }
        return Task.CompletedTask;
    }
}