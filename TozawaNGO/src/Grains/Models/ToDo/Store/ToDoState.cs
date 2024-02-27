
namespace Grains.Models.ToDo.Store
{
    [GenerateSerializer]
    public class ToDoStates
    {
        public HashSet<Guid> Items { get; set; }
    }

    [GenerateSerializer]
    public class ToDoState
    {
        public TodoItem ToDo { get; set; }
    }
}