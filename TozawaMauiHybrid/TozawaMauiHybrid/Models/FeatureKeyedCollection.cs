
using TozawaMauiHybrid.Models.Dtos;

namespace TozawaNGO.Models
{
    public class FeatureKeyedCollection : System.Collections.ObjectModel.KeyedCollection<int, FeatureDto>
    {
        protected override int GetKeyForItem(FeatureDto item) => item.Id;
    }
}