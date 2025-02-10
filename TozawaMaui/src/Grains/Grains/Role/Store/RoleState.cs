
namespace Grains.Models.Role.Store
{
    [GenerateSerializer]
    public class RoleStates
    {
        public HashSet<Guid> Items { get; set; }
    }

    [GenerateSerializer]
    public class RoleState
    {
        public RoleItem Role { get; set; }
    }
}