
namespace Grains.Models.EmailGuid.Store
{
    [GenerateSerializer]
    public class EmailGuidStates
    {
        public HashSet<string> Items { get; set; }
    }

    [GenerateSerializer]
    public class EmailGuidState
    {
        public EmailGuidItem EmailGuid { get; set; }
    }
}