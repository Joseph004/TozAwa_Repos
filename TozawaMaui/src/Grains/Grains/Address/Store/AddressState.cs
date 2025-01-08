
namespace Grains.Models.Address.Store
{
    [GenerateSerializer]
    public class AddressStates
    {
        public HashSet<Guid> Items { get; set; }
    }

    [GenerateSerializer]
    public class AddressState
    {
        public AddressItem Address { get; set; }
    }
}