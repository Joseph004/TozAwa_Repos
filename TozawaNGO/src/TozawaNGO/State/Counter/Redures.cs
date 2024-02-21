using Fluxor;
using TozawaNGO.State.Counter.Actions;

namespace TozawaNGO.State.Counter;

public static class Redures
{
    [ReducerMethod(typeof(IncrementCounterAction))]
    public static CounterState ReduceIncrementCounterAction(CounterState state)
    {
        return new() { ClickCount = state.ClickCount + 1 };
    }
    [ReducerMethod(typeof(StartCounterAction))]
    public static CounterState ReduceStartCounterAction(CounterState state)
    {
        return new() { Stop = false };
    }
    [ReducerMethod(typeof(StopCounterAction))]
    public static CounterState ReduceStopCounterAction(CounterState state)
    {
        return new() { Stop = true };
    }
    [ReducerMethod(typeof(DecrementCounterAction))]
    public static CounterState ReduceDecrementCounterAction(CounterState state)
    {
        return new() { ClickCount = state.ClickCount - 1 };
    }
}