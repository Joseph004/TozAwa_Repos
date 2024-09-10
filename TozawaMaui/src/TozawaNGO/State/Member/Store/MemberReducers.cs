using Fluxor;
using Grains;
using MudBlazor;

namespace TozawaNGO.State.Member.Store;

public static class Redures
{
    [ReducerMethod]
    public static MemberState ReduceHandleSearchStringMemberAction(MemberState state, HandleSearchStringMemberAction action)
    {
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, Members = state.Members, SearchString = action.searchString, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }
    [ReducerMethod(typeof(UnSubscribeAction))]
    public static MemberState ReduceUnSubscribeAction(MemberState state)
    {
        state.Subscription?.UnsubscribeAsync();
        state.HubConnection.DisposeAsync();
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, Members = state.Members, SearchString = state.SearchString, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceFetchDataAction(MemberState state, MemberDataAction action) => new() { IsLoading = true, TotalItems = state.TotalItems, Page = action.page, PageSize = action.pageSize, SearchString = action.searchString, IncludeDeleted = action.includeDeleted, PageOfEmail = action.pageOfEmail, Email = action.email, ScrollTop = action.scrollTop, LoadingState = action.loadingState, JSRuntime = action.jSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };

    [ReducerMethod]
    public static MemberState ReduceDataFetchedAction(MemberState state, MemberDataFechedAction action)
    {
        HandleNotificationAsync(action.notifications, state);
        foreach (var member in action.members)
        {
            if (!state.DescriptionIcon.ContainsKey(member.Id))
            {
                state.DescriptionIcon.Add(member.Id, member.Description.Length > 15 ? Icons.Material.Outlined.Info : "");
                state.MudTextField.Add(member.Id, new MudTextField<string>());
            }
        }
        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, Subscription = action.subscription, TotalItems = action.totalItems, SearchString = state.SearchString, Members = action.members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = action.hubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceHandleAttachments(MemberState state, HandleAttachments action)
    {
        if (state.Members.TryGetValue(action.ownerId, out var current))
        {
            foreach (var item in action.AttachmentDtos)
            {
                if (!action.isDeleted)
                {
                    if (!state.Members[state.Members.IndexOf(current)].Attachments.Any(x => x.Id == item.Id))
                    {
                        state.Members[state.Members.IndexOf(current)].Attachments.Add(item);
                        if (string.IsNullOrEmpty(state.Members[state.Members.IndexOf(current)].Thumbnail))
                        {
                            state.Members[state.Members.IndexOf(current)].Thumbnail = item.Thumbnail;
                        }
                    }
                }
                else
                {
                    if (state.Members[state.Members.IndexOf(current)].Attachments.Any(x => x.Id == item.Id))
                    {
                        state.Members[state.Members.IndexOf(current)].Attachments.RemoveAll(x => x.Id == item.Id);
                        if (string.IsNullOrEmpty(state.Members[state.Members.IndexOf(current)].Thumbnail))
                        {
                            var thb = state.Members[state.Members.IndexOf(current)].Attachments.FirstOrDefault(x => !string.IsNullOrEmpty(x.Thumbnail));
                            if (thb != null)
                            {
                                state.Members[state.Members.IndexOf(current)].Thumbnail = thb.Thumbnail;
                            }
                        }
                    }
                }
            }

        }

        state.LoadingState.SetRequestInProgress(false);
        return new()
        {
            IsLoading = false,
            Subscription = state.Subscription,
            TotalItems = state.TotalItems,
            SearchString = state.SearchString,
            Members = state.Members,
            Page = state.Page,
            PageSize = state.PageSize,
            IncludeDeleted = state.IncludeDeleted,
            PageOfEmail = state.PageOfEmail,
            Email = state.Email,
            HubConnection = state.HubConnection,
            ScrollTop = state.ScrollTop,
            LoadingState = state.LoadingState,
            JSRuntime = state.JSRuntime,
            SelectedItem = state.SelectedItem,
            DescriptionIcon = state.DescriptionIcon,
            MudTextField = state.MudTextField
        };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberPatchAction(MemberState state, MemberPatchAfterAction action)
    {
        if (state.Members.TryGetValue(action.member.Id, out var current))
        {
            if (!action.member.Deleted)
            {
                state.Members[state.Members.IndexOf(current)] = action.member;
                if (!state.DescriptionIcon.ContainsKey(action.member.Id))
                {
                    state.DescriptionIcon.Add(action.member.Id, action.member.Description.Length > 20 ? Icons.Material.Outlined.Info : "");
                    state.MudTextField.Add(action.member.Id, new MudTextField<string>());
                }
            }
            else
            {
                state.Members.RemoveAt(state.Members.IndexOf(current));

                state.DescriptionIcon.Remove(action.member.Id);
            }
        }
        else
        {
            if (!action.member.Deleted)
            {
                state.Members.Add(action.member);
                if (!state.DescriptionIcon.ContainsKey(action.member.Id))
                {
                    state.DescriptionIcon.Add(action.member.Id, action.member.Description.Length > 20 ? Icons.Material.Outlined.Info : "");
                    state.MudTextField.Add(action.member.Id, new MudTextField<string>());
                }
            }
        }

        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberDeletedForeverAction(MemberState state, MemberDeletedForeverAction action)
    {
        if (state.Members.TryGetValue(action.id, out var current))
        {
            state.Members.RemoveAt(state.Members.IndexOf(current));

            state.DescriptionIcon.Remove(action.id);
            state.MudTextField.Remove(action.id);
        }

        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberSelectedAction(MemberState state, MemberSelectedAction action)
    {
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = action.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceScrollTopAction(MemberState state, ScrollTopAction action)
    {
        return new() { IsLoading = false, SearchString = state.SearchString, TotalItems = state.TotalItems, Subscription = state.Subscription, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = action.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberAddAction(MemberState state, MemberAddAfterAction action)
    {
        if (state.Members.TryGetValue(action.member.Id, out var current))
        {
            if (action.member.Timestamp > current.Timestamp)
            {
                state.Members[state.Members.IndexOf(current)] = action.member;
            }
        }
        else
        {
            state.Members.Add(action.member);
        }
        if (!state.DescriptionIcon.ContainsKey(action.member.Id))
        {
            state.DescriptionIcon.Add(action.member.Id, action.member.Description.Length > 20 ? Icons.Material.Outlined.Info : "");
            state.MudTextField.Add(action.member.Id, new MudTextField<string>());
        }
        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, SearchString = null, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }
    private static Task HandleNotificationAsync(List<MemberNotification> notifications, MemberState state)
    {
        foreach (var notification in notifications)
        {
            if (notification.Item == null)
            {
                if (state.Members.Remove(notification.ItemKey))
                {
                }
                return Task.CompletedTask;
            }
            if (state.Members.TryGetValue(notification.Item.UserId, out var current))
            {
                if (notification.Item.Timestamp > current.Timestamp)
                {
                    state.Members[state.Members.IndexOf(current)] = GetMember(notification.Item);
                }
                return Task.CompletedTask;
            }
            state.Members.Add(GetMember(notification.Item));
        }
        return Task.CompletedTask;
    }
    private static TozawaNGO.Models.Dtos.MemberDto GetMember(MemberItem memberItem)
    {
        return new TozawaNGO.Models.Dtos.MemberDto
        {
            Id = memberItem.UserId,
            PartnerId = memberItem.PartnerId,
            Email = memberItem.Email,
            Description = memberItem.Description,
            DescriptionTextId = memberItem.DescriptionTextId,
            FirstName = memberItem.FirstName,
            LastName = memberItem.LastName,
            Deleted = memberItem.Deleted,
            Admin = memberItem.AdminMember
        };
    }
}