using System;
using System.Threading.Tasks;
using Grains;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TozawaNGO.Services;
using TozawaNGO.State.ToDo;
using Fluxor;
using TozawaNGO.Shared;
using TozawaNGO.Models.Dtos;
using TozawaNGO.State.ToDo.Actions;
using TozawaNGO.Helpers;

namespace TozawaNGO.Pages
{
    public partial class Todo : BasePage
    {
        [Inject] IState<ToDoState> ToDoState { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] TodoService TodoService { get; set; }
        private string newTodo;
        protected override async Task OnInitializedAsync()
        {
            newTodo = ToDoState.Value.NewItem;
            await base.OnInitializedAsync();

            Dispatcher.Dispatch(new ToDoDataAction());
            Dispatcher.Dispatch(new TozawaNGO.State.ToDo.Actions.HandleInputTextToDoAction(newTodo));
        }

        public int TotalToDo()
        {
            return ToDoState.Value.Todos.Count(todo => !todo.IsDone);
        }

        public void OnItemEnter(ChangeEventArgs args)
        {
            newTodo = (string)args.Value;
            Dispatcher.Dispatch(new TozawaNGO.State.ToDo.Actions.HandleInputTextToDoAction(newTodo));

            StateHasChanged();
        }
        protected override void Dispose(bool disposed)
        {
            try
            {
                Dispatcher.Dispatch(new TozawaNGO.State.ToDo.Actions.UnSubscribeAction());
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
                Dispatcher.Dispatch(new TozawaNGO.State.ToDo.Actions.ToDoAddAction(newTodo));
                Dispatcher.Dispatch(new TozawaNGO.State.ToDo.Actions.HandleInputTextToDoAction(null));
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

