namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class OrganizationItem : IEquatable<OrganizationItem>
  {
    public OrganizationItem(
   Guid id,
   string name,
   string code,
   string email,
   string phoneNumber,
   string createdBy,
   DateTime createDate,
   string modifiedBy,
   DateTime? modifiedDate,
   List<int> features,
   int attachmentsCount,
   Guid ownerKey,
   string comment,
   Guid commentTextId,
   string description,
   Guid descriptionTextId,
   bool deleted
    )
        : this(
              id,
    name,
    code,
    email,
    phoneNumber,
    createdBy,
    createDate,
    modifiedBy,
   modifiedDate,
   features,
   attachmentsCount,
    ownerKey,
     comment,
    commentTextId,
    description,
    descriptionTextId,
    deleted,
    DateTime.UtcNow)
    {
    }

    protected OrganizationItem(
        Guid id,
   string name,
   string code,
   string email,
   string phoneNumber,
   string createdBy,
   DateTime createDate,
   string modifiedBy,
   DateTime? modifiedDate,
   List<int> features,
   int attachmentsCount,
   Guid ownerKey,
   string comment,
   Guid commentTextId,
   string description,
   Guid descriptionTextId,
   bool deleted
        , DateTime timeStamp)
    {
      Id = id;
      Name = name;
      Code = code;
      Email = email;
      PhoneNumber = phoneNumber;
      CreatedBy = createdBy;
      CreateDate = createDate;
      ModifiedBy = modifiedBy;
      ModifiedDate = modifiedDate;
      Features = features;
      AttachmentsCount = attachmentsCount;
      OwnerKey = ownerKey;
      Deleted = deleted;
      Timestamp = timeStamp;
      Description = description;
      Comment = comment;
      DescriptionTextId = descriptionTextId;
      CommentTextId = commentTextId;
    }

    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Name { get; set; }
    [Id(2)]
    public string Code { get; set; }
    [Id(3)]
    public string Email { get; set; }
    [Id(4)]
    public string PhoneNumber { get; set; }
    [Id(5)]
    public string CreatedBy { get; set; }
    [Id(6)]
    public DateTime CreateDate { get; set; }
    [Id(7)]
    public string ModifiedBy { get; set; }
    [Id(8)]
    public DateTime? ModifiedDate { get; set; }
    [Id(9)]
    public List<int> Features { get; set; }
    [Id(10)]
    public int AttachmentsCount { get; set; }
    [Id(11)]
    public Guid OwnerKey { get; }
    [Id(12)]
    public bool Deleted { get; set; }
    [Id(13)]
    public DateTime Timestamp { get; }
    [Id(14)]
    public string Comment { get; }
    [Id(15)]
    public Guid CommentTextId { get; }
    [Id(16)]
    public string Description { get; }
    [Id(17)]
    public Guid DescriptionTextId { get; }

    public bool Equals(OrganizationItem OrganizationItem)
    {
      if (OrganizationItem == null) return false;
      return
      Id == OrganizationItem.Id
      && Name == OrganizationItem.Name
       && Code == OrganizationItem.Email
       && PhoneNumber == OrganizationItem.PhoneNumber
       && Email == OrganizationItem.Email
       && CreatedBy == OrganizationItem.CreatedBy
       && Description == OrganizationItem.Description
       && Comment == OrganizationItem.Comment
       && DescriptionTextId == OrganizationItem.DescriptionTextId
       && CommentTextId == OrganizationItem.CommentTextId
     && CreateDate == OrganizationItem.CreateDate
     && ModifiedBy == OrganizationItem.ModifiedBy
       && ModifiedDate == OrganizationItem.ModifiedDate
       && Features == OrganizationItem.Features
       && OwnerKey == OrganizationItem.OwnerKey
       && Timestamp == OrganizationItem.Timestamp;
    }
  }
}