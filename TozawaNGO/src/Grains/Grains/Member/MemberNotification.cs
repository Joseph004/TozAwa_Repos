using Orleans.Concurrency;
using System;

namespace Grains
{
    [Immutable]
    public class MemberNotification(Guid itemKey, MemberItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public MemberItem Item { get; } = item;
    }
}