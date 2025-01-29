using Grains.Auth.Models.Dtos.Backend;

namespace Grains
{
    [Immutable]
    public class EmailGuidNotification(string itemKey, EmailGuidItem item)
    {
        public string ItemKey { get; } = itemKey;
        public EmailGuidItem Item { get; } = item;
    }
}