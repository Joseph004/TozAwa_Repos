using Fluxor;

namespace TozawaMauiHybrid.State.Home.Store;

[FeatureState]
public record HomeState
{
    public double ScrollTop { get; init; }
}