
using TozawaMauiHybrid.HttpClients;

namespace TozawaMauiHybrid.Models.FormModels
{
    public class PatchFeatureRequest : PatchBase
    {
        public string Description { get; set; } = null;
        public string Text { get; set; } = null;
        public bool? Deleted { get; set; } = null;
    }
}