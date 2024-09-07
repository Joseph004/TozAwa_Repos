using Orleans.Concurrency;
using System;

namespace Grains
{
    [GenerateSerializer]
    [Immutable]
    public class TodoItem : IEquatable<TodoItem>
    {
        public TodoItem(Guid key, string title, bool isDone, Guid ownerKey)
            : this(key, title, isDone, ownerKey, DateTime.UtcNow)
        {
        }

        protected TodoItem(Guid key, string title, bool isDone, Guid ownerKey, DateTime timestamp)
        {
            Key = key;
            Title = title;
            IsDone = isDone;
            OwnerKey = ownerKey;
            Timestamp = timestamp;
        }

        [Id(0)]
        public Guid Key { get; }
        [Id(1)]
        public string Title { get; }
        [Id(2)]
        public bool IsDone { get; }
        [Id(3)]
        public Guid OwnerKey { get; }
        [Id(4)]
        public DateTime Timestamp { get; }

        public bool Equals(TodoItem other)
        {
            if (other == null) return false;
            return Key == other.Key
                && Title == other.Title
                && IsDone == other.IsDone
                && OwnerKey == other.OwnerKey
                && Timestamp == other.Timestamp;
        }

        public TodoItem WithIsDone(bool isDone) =>
            new(Key, Title, isDone, OwnerKey, DateTime.UtcNow);

        public TodoItem WithTitle(string title) =>
            new(Key, title, IsDone, OwnerKey, DateTime.UtcNow);
    }
}