using Fluxor;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using TozawaMauiHybrid.Models.Dtos;
using TozawaMauiHybrid.Services;
using TozawaNGO.Models;

namespace TozawaMauiHybrid.State.Feature.Store;

[FeatureState]
public record FeatureState
{
    public string Page { get; init; }
    public string PageSize { get; set; }
    public bool IncludeDeleted { get; init; }
    public int Id { get; init; }
    public string SearchString { get; init; }
    public double ScrollTop { get; init; }
    public bool IsLoading { get; init; }
    public bool Initialized { get; init; }
    public int TotalItems { get; set; }
    public HubConnection HubConnection { get; init; }
    public FeatureKeyedCollection Features { get; init; } = [];
    public LoadingState LoadingState { get; init; }
    public IJSRuntime JSRuntime { get; init; }
    public FeatureDto SelectedItem { get; init; }
}