using Fluxor;

namespace TozawaMauiHybrid.State.Feature.Store;

public static class Redures
{
    [ReducerMethod]
    public static FeatureState ReduceHandleSearchStringFeatureAction(FeatureState state, HandleSearchStringFeatureAction action)
    {
        return new() { IsLoading = false, TotalItems = state.TotalItems, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, Features = state.Features, SearchString = action.searchString, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }

    [ReducerMethod]
    public static FeatureState ReduceFetchDataAction(FeatureState state, FeatureDataAction action) => new() { IsLoading = true, TotalItems = state.TotalItems, Page = action.page, PageSize = action.pageSize, SearchString = action.searchString, IncludeDeleted = action.includeDeleted, ScrollTop = action.scrollTop, LoadingState = action.loadingState, JSRuntime = action.jSRuntime, SelectedItem = state.SelectedItem };

    [ReducerMethod]
    public static FeatureState ReduceDataFetchedAction(FeatureState state, FeatureDataFechedAction action)
    {
        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, TotalItems = action.totalItems, SearchString = state.SearchString, Features = action.Features, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, HubConnection = action.hubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }

    [ReducerMethod]
    public static FeatureState ReduceFeaturePatchAction(FeatureState state, FeaturePatchAfterAction action)
    {
        if (state.Features.TryGetValue(action.feature.Id, out var current))
        {
            if (!action.feature.Deleted)
            {
                state.Features.Remove(current);
                state.Features.Add(action.feature);
            }
            else
            {
                state.Features.RemoveAt(state.Features.IndexOf(current));
            }
        }
        else
        {
            if (!action.feature.Deleted)
            {
                state.Features.Add(action.feature);
            }
        }

        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, TotalItems = state.TotalItems, SearchString = state.SearchString, Features = state.Features, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }

    [ReducerMethod]
    public static FeatureState ReduceFeatureSelectedAction(FeatureState state, FeatureSelectedAction action)
    {
        return new() { IsLoading = false, TotalItems = state.TotalItems, SearchString = state.SearchString, Features = state.Features, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = action.SelectedItem };
    }

    [ReducerMethod]
    public static FeatureState ReduceScrollTopAction(FeatureState state, ScrollTopAction action)
    {
        return new() { IsLoading = false, SearchString = state.SearchString, TotalItems = state.TotalItems, Features = state.Features, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, HubConnection = state.HubConnection, ScrollTop = action.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }

    [ReducerMethod]
    public static FeatureState ReduceFeatureAddAction(FeatureState state, FeatureAddAfterAction action)
    {
        if (state.Features.TryGetValue(action.feature.Id, out var current))
        {
            if (action.feature.Timestamp > current.Timestamp)
            {
                state.Features.Remove(current);
                state.Features.Add(action.feature);
            }
        }
        else
        {
            state.Features.Add(action.feature);
        }
        state.LoadingState.SetRequestInProgress(false);
        return new() { IsLoading = false, TotalItems = state.TotalItems, SearchString = null, Features = state.Features, Page = state.Page, PageSize = state.PageSize, IncludeDeleted = state.IncludeDeleted, HubConnection = state.HubConnection, ScrollTop = state.ScrollTop, LoadingState = state.LoadingState, JSRuntime = state.JSRuntime, SelectedItem = state.SelectedItem };
    }
}