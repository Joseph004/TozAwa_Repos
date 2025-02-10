
namespace Grains.Models.Feature.Store
{
    [GenerateSerializer]
    public class FeatureStates
    {
        public HashSet<Guid> Items { get; set; }
    }

    [GenerateSerializer]
    public class FeatureState
    {
        public FeatureItem Feature { get; set; }
    }
}