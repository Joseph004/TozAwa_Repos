

namespace Grains
{
    [Immutable]
    public class AttachmentNotification(Guid itemKey, AttachmentItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public AttachmentItem Item { get; } = item;
    }
}