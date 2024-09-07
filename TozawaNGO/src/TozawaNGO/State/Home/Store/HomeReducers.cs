using Fluxor;
using Grains;
using TozawaNGO.Helpers;
using TozawaNGO.State.Counter.Actions;

namespace TozawaNGO.State.Home.Store;

public static class Redures
{
    [ReducerMethod]
    public static HomeState ReduceScrollTopAction(HomeState state, ScrollTopAction action)
    {
        return new() { ScrollTop = action.scrollTop };
    }
}