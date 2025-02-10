
namespace TozawaMauiHybrid.State.Home.Store;

public record ScrollTopAction(double scrollTop)
{
    public double ScrollTop { get; } = scrollTop;
}
