using Fluxor;
using TozawaNGO.State.Counter.Actions;

namespace TozawaNGO.State.ToDo;

public static class Redures
{
    [ReducerMethod]
    public static ToDoState ReduceIncrementToDoAction(ToDoState state, IncrementCounterAction action)
    {
        return new() { NewItem = state.NewItem };
    }

    [ReducerMethod]
    public static ToDoState ReduceDecrementToDoAction(ToDoState state, DecrementCounterAction action)
    {
        return new() { NewItem = null };
    }
}