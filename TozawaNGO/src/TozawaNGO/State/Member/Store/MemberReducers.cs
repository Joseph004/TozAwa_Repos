using Fluxor;
using Grains;

namespace TozawaNGO.State.Member.Store;

public static class Redures
{
    [ReducerMethod]
    public static MemberState ReduceHandleSearchStringMemberAction(MemberState state, HandleSearchStringMemberAction action)
    {
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, Members = state.Members, SearchString = action.searchString, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }
    [ReducerMethod(typeof(UnSubscribeAction))]
    public static MemberState ReduceUnSubscribeAction(MemberState state)
    {
        state.Subscription?.UnsubscribeAsync();
        state.HubConnection.DisposeAsync();
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, Members = state.Members, SearchString = state.SearchString, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }

    [ReducerMethod]
    public static MemberState ReduceFetchDataAction(MemberState state, MemberDataAction action) => new() { IsLoading = true, TotalItems = state.TotalItems, Page = action.page, PageSize = action.pageSize, SearchString = action.searchString, IncludeDeleted = action.includeDeleted, PageOfEmail = action.pageOfEmail, Email = action.email, ScrollTop = action.scrollTop, LoadingState = action.loadingState, JSRuntime = action.jSRuntime, SelectedItem = state.SelectedItem };

    [ReducerMethod]
    public static MemberState ReduceDataFetchedAction(MemberState state, MemberDataFechedAction action)
    {
        HandleNotificationAsync(action.notifications, state);
        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, Subscription = action.subscription, TotalItems = action.totalItems, SearchString = state.SearchString, Members = action.members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = action.hubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberPatchAction(MemberState state, MemberPatchAfterAction action)
    {
        if (state.Members.TryGetValue(action.member.Id, out var current))
        {
            state.Members[state.Members.IndexOf(current)] = action.member;
        }
        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberSelectedAction(MemberState state, MemberSelectedAction action)
    {
        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = action.SelectedItem };
    }

    [ReducerMethod]
    public static MemberState ReduceScrollTopAction(MemberState state, ScrollTopAction action)
    {
        return new() { IsLoading = false, SearchString = state.SearchString, TotalItems = state.TotalItems, Subscription = state.Subscription, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = action.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
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

        return new() { IsLoading = false, Subscription = state.Subscription, TotalItems = state.TotalItems, SearchString = null, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
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