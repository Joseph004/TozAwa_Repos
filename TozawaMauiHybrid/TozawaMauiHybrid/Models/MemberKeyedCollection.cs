
using TozawaMauiHybrid.Models.Dtos;

namespace TozawaNGO.Models
{
    public class MemberKeyedCollection : System.Collections.ObjectModel.KeyedCollection<Guid, MemberDto>
    {
        protected override Guid GetKeyForItem(MemberDto item) => item.Id;
    }
}