using Grains.Auth.Models.Authentication;
using Grains.Models;

namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class FeatureItem : IEquatable<FeatureItem>
  {
    public FeatureItem(
  int id,
   Guid textId,
   Guid descriptionTextId,
   bool deleted,
   Guid ownerKey
    )
        : this(
             id,
    textId,
    descriptionTextId,
    deleted,
    ownerKey,
    DateTime.UtcNow)
    {
    }

    protected FeatureItem(
        int id,
   Guid textId,
   Guid descriptionTextId,
   bool deleted,
   Guid ownerKey
        , DateTime timeStamp)
    {
      Id = id;
      TextId = textId;
      DescriptionTextId = descriptionTextId;
      Deleted = deleted;
      OwnerKey = ownerKey;
      Timestamp = timeStamp;
    }

    [Id(0)]
    public int Id { get; set; }
    [Id(1)]
    public Guid TextId { get; set; }
    [Id(2)]
    public Guid DescriptionTextId { get; set; }
    [Id(3)]
    public bool Deleted { get; set; }
    [Id(4)]
    public Guid OwnerKey { get; }
    [Id(5)]
    public DateTime Timestamp { get; }

    public bool Equals(FeatureItem FeatureItem)
    {
      if (FeatureItem == null) return false;
      return
      Id == FeatureItem.Id
      && TextId == FeatureItem.TextId
       && DescriptionTextId == FeatureItem.DescriptionTextId
       && Deleted == FeatureItem.Deleted
       && OwnerKey == FeatureItem.OwnerKey
       && Timestamp == FeatureItem.Timestamp;
    }
  }
}