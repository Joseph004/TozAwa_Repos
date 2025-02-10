namespace Grains
{
    [Immutable]
    public class LoggedInNotification(Guid itemKey, LoggedInItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public LoggedInItem Item { get; } = item;
    }
}