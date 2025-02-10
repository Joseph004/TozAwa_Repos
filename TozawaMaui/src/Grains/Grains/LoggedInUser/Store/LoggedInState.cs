
namespace Grains.Models.LoggedInState.Store
{
    [GenerateSerializer]
    public class LoggedInStates
    {
        public HashSet<Guid> Items { get; set; }
    }

    [GenerateSerializer]
    public class LoggedInState
    {
        public LoggedInItem LoggedIn { get; set; }
    }
}