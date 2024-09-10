using Grains;

namespace TozawaNGO.Models.Dtos
{
    public class MemberKeyedCollection : System.Collections.ObjectModel.KeyedCollection<Guid, MemberDto>
    {
        protected override Guid GetKeyForItem(MemberDto item) => item.Id;
    }
}