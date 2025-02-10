using Fluxor;
using MudBlazor;
using TozawaMauiHybrid.Models.Dtos;

namespace TozawaMauiHybrid.State.Member.Store;

public static class Redures
{
    [ReducerMethod]
    public static MemberState ReduceHandleSearchStringMemberAction(MemberState state, HandleSearchStringMemberAction action)
    {
        return new() { IsLoading = false, TotalItems = state.TotalItems, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, Members = state.Members, SearchString = action.searchString, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceFetchDataAction(MemberState state, MemberDataAction action) => new() { IsLoading = true, TotalItems = state.TotalItems, Page = action.page, PageSize = action.pageSize, SearchString = action.searchString, IncludeDeleted = action.includeDeleted, PageOfEmail = action.pageOfEmail, Email = action.email, ScrollTop = action.scrollTop, LoadingState = action.loadingState, JSRuntime = action.jSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };

    [ReducerMethod]
    public static MemberState ReduceDataFetchedAction(MemberState state, MemberDataFechedAction action)
    {
        foreach (var member in action.members)
        {
            if (!state.DescriptionIcon.ContainsKey(member.Id))
            {
                state.DescriptionIcon.Add(member.Id, member.Description.Length > 15 ? Icons.Material.Outlined.Info : "");
                state.MudTextField.Add(member.Id, new MudTextField<string>());
            }
        }
        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, TotalItems = action.totalItems, SearchString = state.SearchString, Members = action.members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = action.hubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceHandleAttachments(MemberState state, HandleAttachments action)
    {
        if (state.Members.TryGetValue(action.ownerId, out var current))
        {
            foreach (var item in action.AttachmentDtos)
            {
                var files = new OwnerAttachments
                {
                    OwnerId = current.Id,
                    Attachments = [item]
                };
                if (!action.isDeleted)
                {
                    action.attachmentService.SetNotifyChange(files, false);
                    state.Members[state.Members.IndexOf(current)].AttachmentsCount++;
                }
                else
                {
                    action.attachmentService.SetNotifyChange(files, true);
                    state.Members[state.Members.IndexOf(current)].AttachmentsCount--;
                }
            }

        }

        state.LoadingState.SetRequestInProgress(false);
        return new()
        {
            IsLoading = false,
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
                state.Members.Remove(current);
                state.Members.Add(action.member);
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
        return new() { IsLoading = false, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
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
        return new() { IsLoading = false, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberSelectedAction(MemberState state, MemberSelectedAction action)
    {
        return new() { IsLoading = false, TotalItems = state.TotalItems, SearchString = state.SearchString, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = action.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceScrollTopAction(MemberState state, ScrollTopAction action)
    {
        return new() { IsLoading = false, SearchString = state.SearchString, TotalItems = state.TotalItems, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = action.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }

    [ReducerMethod]
    public static MemberState ReduceMemberAddAction(MemberState state, MemberAddAfterAction action)
    {
        if (state.Members.TryGetValue(action.member.Id, out var current))
        {
            if (action.member.Timestamp > current.Timestamp)
            {
                state.Members.Remove(current);
                state.Members.Add(action.member);
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
        return new() { IsLoading = false, TotalItems = state.TotalItems, SearchString = null, Members = state.Members, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, PageOfEmail = state.PageOfEmail, Email = state.Email, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem, DescriptionIcon = state.DescriptionIcon, MudTextField = state.MudTextField };
    }
}