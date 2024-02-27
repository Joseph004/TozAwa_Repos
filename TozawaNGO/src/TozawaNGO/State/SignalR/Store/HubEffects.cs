using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TozawaNGO.State.ToDo.Store;

namespace TozawaNGO.State.SignalR.Store
{
    public class HubEffects(NavigationManager navigationManager)
    {
        private readonly HubConnection _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/hubs/clienthub"))
                .WithAutomaticReconnect()
                .Build();

        [EffectMethod]
        public Task OnToDoChanged(HubToDoChangedAction action, IDispatcher dispatcher)
        {
            switch (action.ChangeType)
            {
                case HubChangeType.Added:
                    if (action.Id != null)
                    {
                        dispatcher.Dispatch(new LoadItemAction(action.Id.Value));
                    }
                    break;
                case HubChangeType.Updated:
                    if (action.Id != null)
                    {
                        dispatcher.Dispatch(new LoadItemAction(action.Id.Value));
                    }
                    break;
                case HubChangeType.Deleted:
                    if (action.Id != null)
                    {
                        dispatcher.Dispatch(new RemoveItemAction(action.Id.Value));
                    }
                    break;
                case HubChangeType.NeedReload:
                    dispatcher.Dispatch(new LoadDataAction());
                    break;
            }

            return Task.CompletedTask;
        }

        [EffectMethod(typeof(HubStartAction))]
        public async Task Start(IDispatcher dispatcher)
        {
            await _hubConnection.StartAsync();

            _hubConnection.Reconnecting += (ex) =>
            {
                dispatcher.Dispatch(new HubSetConnectedAction(false));
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                dispatcher.Dispatch(new HubSetConnectedAction(true));
                return Task.CompletedTask;
            };

            _hubConnection.On<Guid>("ToDoAdded", (id) => dispatcher.Dispatch(new HubToDoChangedAction(HubChangeType.Added, id)));
            _hubConnection.On<Guid>("ToDoUpdated", (id) => dispatcher.Dispatch(new HubToDoChangedAction(HubChangeType.Updated, id)));
            _hubConnection.On<Guid>("ToDoDeleted", (id) => dispatcher.Dispatch(new HubToDoChangedAction(HubChangeType.Deleted, id)));
            _hubConnection.On("ToDoChanged", () => dispatcher.Dispatch(new HubToDoChangedAction(HubChangeType.NeedReload)));

            dispatcher.Dispatch(new HubSetConnectedAction(true));
        }
    }
}