using System;
using Grains;

namespace TozawaNGO.Models.Dtos
{
    public class TodoKeyedCollection : System.Collections.ObjectModel.KeyedCollection<Guid, TodoItem>
    {
        protected override Guid GetKeyForItem(TodoItem item) => item.Key;
    }
}