namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class EmailGuidItem : IEquatable<EmailGuidItem>
  {
    public EmailGuidItem(
  Guid id,
  string email,
   Guid ownerKey
    )
        : this(
             id,
             email,
    ownerKey,
    DateTime.UtcNow)
    {
    }

    protected EmailGuidItem(
        Guid id,
        string email,
   Guid ownerKey
        , DateTime timeStamp)
    {
      Id = id;
      Email = email;
      OwnerKey = ownerKey;
      Timestamp = timeStamp;
    }

    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Email { get; set; }
    [Id(2)]
    public Guid OwnerKey { get; }
    [Id(3)]
    public DateTime Timestamp { get; }

    public bool Equals(EmailGuidItem EmailGuidItem)
    {
      if (EmailGuidItem == null) return false;
      return
      Id == EmailGuidItem.Id
       && Email == EmailGuidItem.Email
       && OwnerKey == EmailGuidItem.OwnerKey
       && Timestamp == EmailGuidItem.Timestamp;
    }
  }
}