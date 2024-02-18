using Fluxor;
using TozawaNGO.State.Counter.Actions;

namespace TozawaNGO.State.Counter;

public static class Redures
{
    [ReducerMethod]
    public static CounterState ReduceIncrementCounterAction(CounterState state, IncrementCounterAction action)
    {
        return new() { ClickCount = state.ClickCount + 1 };
    }
    [ReducerMethod]
    public static CounterState ReduceStartCounterAction(CounterState state, StartCounterAction action)
    {
        return new() { Stop = false };
    }
    [ReducerMethod]
    public static CounterState ReduceStopCounterAction(CounterState state, StopCounterAction action)
    {
        return new() { Stop = true };
    }
    [ReducerMethod]
    public static CounterState ReduceDecrementCounterAction(CounterState state, IncrementCounterAction action)
    {
        return new() { ClickCount = state.ClickCount - 1 };
    }
}