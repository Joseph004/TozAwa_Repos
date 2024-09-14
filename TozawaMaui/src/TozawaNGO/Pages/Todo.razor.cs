using Grains;
using Microsoft.AspNetCore.Components;
using TozawaNGO.Services;
using Fluxor;
using TozawaNGO.Shared;
using TozawaNGO.State.ToDo.Store;
using Microsoft.JSInterop;
using TozawaNGO.StateHandler;
using ShareRazorClassLibrary.Services;

namespace TozawaNGO.Pages
{
    public partial class Todo : BasePage
    {
        [Inject] IState<TozawaNGO.State.ToDo.Store.ToDoState> ToDoState { get; set; }
        [Inject] IDispatcher Dispatcher { get; set; }
        [Inject] TodoService TodoService { get; set; }
        [Inject] LoadingState LoadingState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ScrollTopState ScrollTopState { get; set; }
        private string newTodo;
        private bool firstLoaded;
        private double scrollTop;

        private void SetScroll()
        {
            Dispatcher.Dispatch(new ScrollTopAction(ScrollTopState.ScrollTop[ScrollTopState.Source]));
        }
        protected override async Task OnInitializedAsync()
        {
            ScrollTopState.SetSource("todoPage");
            LoadingState.SetRequestInProgress(true);
            ScrollTopState.OnChange += SetScroll;
            newTodo = ToDoState.Value.NewItem;
            ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double scroll);
            scrollTop = scroll;

            await base.OnInitializedAsync();

            Dispatcher.Dispatch(new ToDoDataAction(ScrollTopState.ScrollTop.TryGetValue(ScrollTopState.Source, out double value) ? value : 0));
        }
        private int Count = 0;
        private async Task SetScrollJS()
        {
            if (Count != 0) return;
            if (scrollTop != 0)
            {
                Count++;
                await JSRuntime.InvokeAsync<object>("SetScroll", (-1) * scrollTop);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                firstLoaded = true;
                LoadingState.SetRequestInProgress(true);
            }
            if (!ToDoState.Value.IsLoading && ToDoState.Value.Todos.Count > 0 && firstLoaded)
            {
                firstLoaded = false;
                LoadingState.SetRequestInProgress(false);
                await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.5))).ContinueWith(async o => { await SetScrollJS(); });
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

