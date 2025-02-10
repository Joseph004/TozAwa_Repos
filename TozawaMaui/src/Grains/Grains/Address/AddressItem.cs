namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class AddressItem : IEquatable<AddressItem>
  {
    public AddressItem(
  Guid id,
   string name,
   string address,
   string city,
   string state,
   string country,
   string zipCode,
   bool active,
   Guid ownerKey
    )
        : this(
            id,
    name,
    address,
    city,
    state,
    country,
    zipCode,
    active,
    ownerKey,
    DateTime.UtcNow)
    {
    }

    protected AddressItem(
        Guid id,
   string name,
   string address,
   string city,
   string state,
   string country,
   string zipCode,
   bool active,
   Guid ownerKey
        , DateTime timeStamp)
    {
      Id = id;
      Name = name;
      Address = address;
      City = city;
      State = state;
      Country = country;
      ZipCode = zipCode;
      Active = active;
      OwnerKey = ownerKey;
      Timestamp = timeStamp;
    }

    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Name { get; set; }
    [Id(2)]
    public string Address { get; set; }
    [Id(3)]
    public string City { get; set; }
    [Id(4)]
    public string State { get; set; }
    [Id(5)]
    public string Country { get; set; }
    [Id(6)]
    public string ZipCode { get; set; }
    [Id(7)]
    public bool Active { get; set; }
    [Id(8)]
    public Guid OwnerKey { get; }
    [Id(9)]
    public DateTime Timestamp { get; }

    public bool Equals(AddressItem AddressItem)
    {
      if (AddressItem == null) return false;
      return
      Id == AddressItem.Id
      && Name == AddressItem.Name
       && Address == AddressItem.Address
       && City == AddressItem.State
       && State == AddressItem.State
       && Country == AddressItem.Country
     && ZipCode == AddressItem.ZipCode
     && Active == AddressItem.Active
       && OwnerKey == AddressItem.OwnerKey
       && Timestamp == AddressItem.Timestamp;
    }
  }
}