using Fluxor;
using Grains;
using TozawaNGO.Services;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using ShareRazorClassLibrary.Services;
using ShareRazorClassLibrary.Models.Dtos;
using ShareRazorClassLibrary.Models.Requests;
using ShareRazorClassLibrary.Models.FormModels;
using Grains.Helpers;

namespace TozawaNGO.State.Member.Store;

public class Effects(MemberService memberService, AttachmentService attachmentService)
{
    [EffectMethod]
    public async Task HandleMemberDataAction(MemberDataAction action, IDispatcher dispatcher)
    {
        List<MemberNotification> notifications = [];
        var subscription = await memberService.SubscribeAsync(SystemTextId.MemberOwnerId, notification => Task.Run(() =>
             HandleNotificationAsync(notifications, notification)));

        var members = new Models.MemberKeyedCollection();
        var data = await memberService.GetItems(action.page, action.pageSize, action.includeDeleted, action.searchString, action.pageOfEmail, action.email);

        var entity = data.Entity ?? new TableData<MemberDto>();
        var items = entity.Items ?? [];

        foreach (var item in items)
        {
            members.Add(item);
        }

        var hubConnection = await StartHubConnection();
        AddMemberDataListener(hubConnection, dispatcher);
        UpdateMemberDataListener(hubConnection, dispatcher);
        AddAttachmentDataListener(hubConnection, dispatcher);
        DeletedAttachmentDataListener(hubConnection, dispatcher);
        dispatcher.Dispatch(new MemberDataFechedAction(members, subscription, notifications, entity.TotalItems, hubConnection));
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
    public async Task HandlePatchAction(MemberPatchAction action, IDispatcher dispatcher)
    {
        var updateResponse = await memberService.PatchMember(action.Id, action.Request);
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

    private static void AddMemberDataListener(HubConnection hubConnection, IDispatcher dispatcher)
    {
        hubConnection.On<Guid>("MemberAdded", (id) =>
        dispatcher.Dispatch(new LoadItemAction(id, false)));
    }
    private static void AddAttachmentDataListener(HubConnection hubConnection, IDispatcher dispatcher)
    {
        hubConnection.On<string, Guid, string>("AttachmentAdded", (ids, ownerId, source) =>
        dispatcher.Dispatch(new AttachmentHandleAction(ids, ownerId, source)));
    }
    private static void DeletedAttachmentDataListener(HubConnection hubConnection, IDispatcher dispatcher)
    {
        hubConnection.On<string, Guid, string>("AttachmentDeleted", (ids, ownerId, source) =>
        dispatcher.Dispatch(new AttachmentHandleAction(ids, ownerId, source, true)));
    }
    private static void UpdateMemberDataListener(HubConnection hubConnection, IDispatcher dispatcher)
    {
        hubConnection.On<Guid, bool>("MemberUpdated", (id, isDeletedForever) =>
        dispatcher.Dispatch(new LoadItemAction(id, true, isDeletedForever)));
    }

    [EffectMethod]
    public async Task OnLoadItem(LoadItemAction action, IDispatcher dispatcher)
    {
        if (action.IsDeletedForever)
        {
            dispatcher.Dispatch(new MemberDeletedForeverAction(action.Id));
        }
        else
        {
            var memberResponse = await memberService.GetItem(action.Id);

            if (!memberResponse.Success)
            {
            }
            if (action.IsUpdated)
            {
                dispatcher.Dispatch(new MemberPatchAfterAction(memberResponse.Entity ?? new MemberDto()));
            }
            else
            {
                dispatcher.Dispatch(new MemberAddAfterAction(memberResponse.Entity ?? new MemberDto()));
            }
        }
    }

    [EffectMethod]
    public async Task OnAttachmentHandled(AttachmentHandleAction action, IDispatcher dispatcher)
    {
        if (!string.IsNullOrEmpty(action.Source) && action.Source == nameof(MemberDto))
        {
            var attachments = new List<FileAttachmentDto>();
            if (action.IsDeleted)
            {
                attachments = action.Ids.Select(x => new FileAttachmentDto
                {
                    Id = x
                }).ToList();
            }
            else
            {
                var attachResponse = await attachmentService.GetAttachments(new GetAttachments { AttachmentIds = action.Ids });
                if (!attachResponse.Success)
                {
                    return;
                }
                attachments = attachResponse.Entity ?? [];
            }

            dispatcher.Dispatch(new HandleAttachments(attachments, action.OwnerId, action.IsDeleted));
        }
    }

    [EffectMethod]
    public async Task HandleMemberAddAction(MemberAddAction action, IDispatcher dispatcher)
    {
        var request = new ShareRazorClassLibrary.Models.FormModels.AddMemberRequest
        {
            Email = "",
            FirstName = "",
            LastName = "",
            Description = "",
            DescriptionTranslations = []
        };
        var memberResponse = await memberService.AddMember(request);
        if (!memberResponse.Success)
        {
        }
        dispatcher.Dispatch(new MemberAddAfterAction(memberResponse.Entity ?? new MemberDto()));
    }

    private static Task HandleNotificationAsync(List<MemberNotification> notifications, MemberNotification notification)
    {
        if (!notifications.Any(x => x.ItemKey == notification.ItemKey))
        {
            notifications.Add(notification);
        }
        return Task.CompletedTask;
    }
}