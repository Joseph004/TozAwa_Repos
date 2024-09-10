using Orleans.Concurrency;
using System;

namespace Grains
{
    [Immutable]
    public class TranslationNotification(Guid itemKey, TranslationItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public TranslationItem Item { get; } = item;
    }
}