using Fluxor;
using Microsoft.AspNetCore.SignalR.Client;

namespace TozawaNGO.State.Home.Store;

[FeatureState]
public record HomeState
{
    public double ScrollTop { get; init; }
}