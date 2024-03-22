using Fluxor;
using Grains;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using TozawaNGO.Models.Dtos;
using TozawaNGO.Services;

namespace TozawaNGO.State.Member.Store;

[FeatureState]
public record MemberState
{
    public string Page { get; init; }
    public string PageSize { get; set; }
    public bool IncludeDeleted { get; init; }
    public string SearchString { get; init; }
    public string PageOfEmail { get; init; }
    public string Email { get; init; }
    public double ScrollTop { get; init; }
    public bool IsLoading { get; init; }
    public bool Initialized { get; init; }
    public int TotalItems { get; set; }
    public HubConnection HubConnection { get; init; }
    public MemberKeyedCollection Members { get; init; } = [];
    public Orleans.Streams.StreamSubscriptionHandle<MemberNotification> Subscription { get; init; }
    public LoadingState LoadingState { get; init; }
    public IJSRuntime JSRuntime { get; init; }
    public MemberDto SelectedItem { get; set; }
    public MudTable<MemberDto> Table { get; set; }
}