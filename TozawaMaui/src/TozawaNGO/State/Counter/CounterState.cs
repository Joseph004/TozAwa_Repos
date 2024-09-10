using Fluxor;

namespace TozawaNGO.State.Counter;

[FeatureState]
public record CounterState
{
    public bool Stop { get; init; }
    public int ClickCount { get; init; }
}