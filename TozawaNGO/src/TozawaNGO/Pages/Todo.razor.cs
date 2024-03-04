using Grains;
using Microsoft.AspNetCore.Components;
using TozawaNGO.Services;
using Fluxor;
using TozawaNGO.Shared;
using TozawaNGO.State.ToDo.Store;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.StateHandler;

namespace TozawaNGO.Pages
{
    public partial class Todo : BasePage
    {
        [Inject] IState<ToDoState> ToDoState { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] TodoService TodoService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ScrollTopState ScrollTopState { get; set; }
        private string newTodo;

        private void SetScroll()
        {
            Dispatcher.Dispatch(ScrollTopState.ScrollTop);
        }
        protected override async Task OnInitializedAsync()
        {
            ScrollTopState.OnChange += SetScroll;
            newTodo = ToDoState.Value.NewItem;
            await base.OnInitializedAsync();

            Dispatcher.Dispatch(new ToDoDataAction());
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void OnItemEnter(ChangeEventArgs args)
        {
            newTodo = (string)args.Value;
            Dispatcher.Dispatch(new HandleInputTextToDoAction(newTodo));

            StateHasChanged();
        }
        protected override void Dispose(bool disposed)
        {
            try
            {
                ScrollTopState.OnChange -= SetScroll;
                Dispatcher.Dispatch(new UnSubscribeAction());
            }
            catch
            {
            }
            base.Dispose(disposed);
        }

        private async Task AddTodoAsync()
        {
            if (!string.IsNullOrWhiteSpace(newTodo))
            {
                Dispatcher.Dispatch(new ToDoAddAction(newTodo));
                Dispatcher.Dispatch(new HandleInputTextToDoAction(null));
                newTodo = ToDoState.Value.NewItem;
                await Task.CompletedTask;
                StateHasChanged();
            }
        }
        private void TryUpdateCollection(TodoItem item)
        {
            if (ToDoState.Value.Todos.TryGetValue(item.Key, out var current))
            {
                if (item.Timestamp > current.Timestamp)
                {
                    ToDoState.Value.Todos[ToDoState.Value.Todos.IndexOf(current)] = item;
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
            ToDoState.Value.Todos.Remove(item.Key);
        }
    }
}

