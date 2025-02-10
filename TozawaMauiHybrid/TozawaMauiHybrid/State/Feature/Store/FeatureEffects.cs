using Fluxor;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Models.FormModels;
using TozawaMauiHybrid.Services;
using TozawaNGO.Models;

namespace TozawaMauiHybrid.State.Feature.Store;

public class Effects(FeatureService featureService)
{
    [EffectMethod]
    public async Task HandleFeatureDataAction(FeatureDataAction action, Fluxor.IDispatcher dispatcher)
    {
        var Features = new FeatureKeyedCollection();
        var data = await featureService.GetItems(action.page, action.pageSize, action.includeDeleted, action.searchString, action.id.ToString().Trim());

        var entity = data.Entity ?? new TableData<FeatureDto>();
        var items = entity.Items ?? [];
        items = [
           new() {
                 Id = 125,
                 Deleted = false,
                 Text = "fgfghffjfgjgfjfgjgfjgfngfjhgfhfghgfhghghdghdhghgdhgdhdhdghgdhgdhdhgnhghdghdhdfgrsgfdsfgfsgohfuohgrogrsoghsuoguhgoghougbruhggbrouruhrukrhgrgurgrughrugugugeuogaugdughdaulghaulhgeauhealugealugedeudguhgufgdfughfuhfhfhgfi",
                 Description = "dhfdgdfsfgdsgfdsgdzcvfdgrgrsgfsgfsggdjgdbcxfvfshgdhdfzvfxhghnfxvfsdhthfdgdfghfdgdfsgfsgsfgfsgfhfsdbgfdjtgdhfbhuohgeuogeuogeauogouguogouhgoghghoghroghrghrghroghrohgrgrghrghrhgrghrlugrlughrhgruhgrughruhgruhgrouhgrughrughrughuhgruhgruhguhuoghuohguhger"
            },
            new() {
                 Id = 12,
                 Deleted = true,
                 Text = "AddReference",
                 Description = "this is a description"
            },
        ];

        foreach (var item in items)
        {
            Features.Add(item);
        }

        var hubConnection = await StartHubConnection();
        AddFeatureDataListener(hubConnection, dispatcher);
        UpdateFeatureDataListener(hubConnection, dispatcher);
        dispatcher.Dispatch(new FeatureDataFechedAction(Features, entity.TotalItems, hubConnection));
        await Task.Delay(new TimeSpan(0, 0, Convert.ToInt32(0.5))).ContinueWith(async o =>
        {
            if (action.scrollTop != 0)
            {
                var objs = new List<object>()
            {
                (-1) * action.scrollTop
            };
                await action.jSRuntime.InvokeAsync<object>("SetScroll", [.. objs]);
            }
        });
    }
    [EffectMethod]
    public async Task HandlePatchAction(FeaturePatchAction action, Fluxor.IDispatcher dispatcher)
    {
        var updateResponse = await featureService.PatchFeature(action.Id, action.Request);
        if (updateResponse.Success)
        {
            /// Handle feedback message
        }
    }
    private static async Task<HubConnection> StartHubConnection()
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

    private static void AddFeatureDataListener(HubConnection hubConnection, Fluxor.IDispatcher dispatcher)
    {
        hubConnection.On<int>("FeatureAdded", (id) =>
        dispatcher.Dispatch(new LoadItemAction(id, false)));
    }
    private static void UpdateFeatureDataListener(HubConnection hubConnection, Fluxor.IDispatcher dispatcher)
    {
        hubConnection.On<int, bool>("FeatureUpdated", (id, isDeletedForever) =>
        dispatcher.Dispatch(new LoadItemAction(id, true)));
    }

    [EffectMethod]
    public async Task OnLoadItem(LoadItemAction action, Fluxor.IDispatcher dispatcher)
    {
        var FeatureResponse = await featureService.GetItem(action.Id);

        if (!FeatureResponse.Success)
        {
        }
        if (action.IsUpdated)
        {
            dispatcher.Dispatch(new FeaturePatchAfterAction(FeatureResponse.Entity ?? new FeatureDto()));
        }
        else
        {
            dispatcher.Dispatch(new FeatureAddAfterAction(FeatureResponse.Entity ?? new FeatureDto()));
        }
    }

    [EffectMethod]
    public async Task HandleFeatureAddAction(FeatureAddAction action, Fluxor.IDispatcher dispatcher)
    {
        var request = new AddFeatureRequest
        {
            Text = "",
            Description = "",
            TextTranslations = [],
            DescriptionTranslations = []
        };
        var FeatureResponse = await featureService.AddFeature(request);
        if (!FeatureResponse.Success)
        {
        }
        dispatcher.Dispatch(new FeatureAddAfterAction(FeatureResponse.Entity ?? new FeatureDto()));
    }
}