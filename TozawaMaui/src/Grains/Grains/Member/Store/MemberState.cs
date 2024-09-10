
namespace Grains.Models.Member.Store
{
    [GenerateSerializer]
    public class MemberStates
    {
        public HashSet<Guid> Items { get; set; }
    }

    [GenerateSerializer]
    public class MemberState
    {
        public MemberItem Member { get; set; }
    }
}