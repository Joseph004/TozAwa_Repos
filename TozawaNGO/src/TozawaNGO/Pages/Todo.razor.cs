using System;
using System.Threading.Tasks;
using Grains;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaNGO.Services;
using TozawaNGO.State.ToDo;
using Fluxor;
using TozawaNGO.Shared;

namespace TozawaNGO.Pages
{
    public partial class Todo : BasePage
    {
        [Inject] IState<ToDoState> ToDoState { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] TodoService TodoService { get; set; }
        private Guid ownerKey = Guid.Empty;
        private TodoKeyedCollection todos = new();
        //private string newTodo;
        private Orleans.Streams.StreamSubscriptionHandle<TodoNotification> subscription;
        protected override async Task OnInitializedAsync()
        {
            subscription = await TodoService.SubscribeAsync(ownerKey, notification => InvokeAsync(() =>

            HandleNotificationAsync(notification)));
            foreach (var item in await TodoService.GetAllAsync(ownerKey))
            {
                todos.Add(item);
            }
            await base.OnInitializedAsync();
        }

        public void OnItemEnter()
        {
            Dispatcher.Dispatch(new TozawaNGO.State.ToDo.Actions.IncrementToDoAction());

            StateHasChanged();
        }
        protected override void Dispose(bool disposed)
        {
            try
            {
                subscription?.UnsubscribeAsync();
            }
            catch
            {
            }
            base.Dispose();
        }

        private async Task AddTodoAsync()
        {
            if (!string.IsNullOrWhiteSpace(ToDoState.Value.NewItem))
            {
                var todo = new TodoItem(Guid.NewGuid(), ToDoState.Value.NewItem, false, ownerKey);

                await TodoService.SetAsync(todo);
                if (todos.TryGetValue(todo.Key, out var current))
                {
                    if (todo.Timestamp > current.Timestamp)
                    {
                        todos[todos.IndexOf(current)] = todo;
                    }
                }
                else
                {
                    todos.Add(todo);
                }
                Dispatcher.Dispatch(new TozawaNGO.State.ToDo.Actions.DecrementToDoAction());

                StateHasChanged();
            }
        }
        private Task HandleNotificationAsync(TodoNotification notification)
        {
            if (notification.Item == null)
            {
                if (todos.Remove(notification.ItemKey))
                {
                    StateHasChanged();
                }
                return Task.CompletedTask;
            }
            if (todos.TryGetValue(notification.Item.Key, out var current))
            {
                if (notification.Item.Timestamp > current.Timestamp)
                {
                    todos[todos.IndexOf(current)] = notification.Item;
                    StateHasChanged();
                }
                return Task.CompletedTask;
            }
            todos.Add(notification.Item);
            StateHasChanged();
            return Task.CompletedTask;
        }
        private void TryUpdateCollection(TodoItem item)
        {
            if (todos.TryGetValue(item.Key, out var current))
            {
                if (item.Timestamp > current.Timestamp)
                {
                    todos[todos.IndexOf(current)] = item;
                }
            }
        }
        private async Task HandleTodoDoneAsync(ChangeEventArgs args, TodoItem item)
        {
            bool isDone = (bool)args.Value;
            var updated = item.WithIsDone(isDone);
            await TodoService.SetAsync(updated);
            TryUpdateCollection(updated);
        }
        private async Task HandleTodoTitleChangeAsync(ChangeEventArgs args, TodoItem item)
        {
            string title = (string)args.Value;
            var updated = item.WithTitle(title);
            await TodoService.SetAsync(updated);
            TryUpdateCollection(updated);
        }
        private async Task HandleDeleteTodoAsync(TodoItem item)
        {
            await TodoService.DeleteAsync(item.Key);
            todos.Remove(item.Key);
        }
        private class TodoKeyedCollection : System.Collections.ObjectModel.KeyedCollection<Guid, TodoItem>
        {
            protected override Guid GetKeyForItem(TodoItem item) => item.Key;
        }
    }
}

