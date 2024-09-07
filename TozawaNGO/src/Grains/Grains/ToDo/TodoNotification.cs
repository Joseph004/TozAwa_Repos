using Orleans.Concurrency;
using System;

namespace Grains
{
    [Immutable]
    public class TodoNotification(Guid itemKey, TodoItem item)
    {
        public Guid ItemKey { get; } = itemKey;
        public TodoItem Item { get; } = item;
    }
}