using Grains.Auth.Models.Authentication;
using Grains.Helpers;

namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class MemberItem : IEquatable<MemberItem>
  {
    public MemberItem(
    Guid userId,
    Guid partnerId,
    string description,
    Guid descriptionTextId,
     string firstName,
     string lastName,
   string lastLoginCountry,
   string lastLoginCity,
    string lastLoginState,
  string lastLoginIPAdress,
    string adress,
    string userPasswordHash,
    List<Role> roles,
    DateTime lastAttemptLogin,
   string refreshToken,
    DateTime refreshTokenExpiryTime,
    string userCountry,
    bool deleted,
    bool adminMember,
   DateTime? lastLogin,
    string createdBy,
   DateTime createDate,
   string modifiedBy,
  DateTime? modifiedDate,
   List<Guid> stationIds,
  string email,
   string passwordHash,
    int attachmentsCount,
    List<Guid> tenants,
    List<Guid> landLords,
    Guid ownerKey
    )
        : this(
            userId,
    partnerId,
    description,
    descriptionTextId,
     firstName,
     lastName,
   lastLoginCountry,
   lastLoginCity,
    lastLoginState,
  lastLoginIPAdress,
    adress,
    userPasswordHash,
    roles,
    lastAttemptLogin,
   refreshToken,
    refreshTokenExpiryTime,
    userCountry,
    deleted,
    adminMember,
   lastLogin,
    createdBy,
   createDate,
   modifiedBy,
  modifiedDate,
   stationIds,
  email,
   passwordHash,
   attachmentsCount,
   tenants,
   landLords,
    ownerKey,
    DateTime.UtcNow)
    {
    }

    protected MemberItem(
        Guid userId,
    Guid partnerId,
    string description,
    Guid descriptionTextId,
     string firstName,
     string lastName,
   string lastLoginCountry,
   string lastLoginCity,
    string lastLoginState,
  string lastLoginIPAdress,
    string adress,
    string userPasswordHash,
    List<Role> roles,
    DateTime lastAttemptLogin,
   string refreshToken,
    DateTime refreshTokenExpiryTime,
    string userCountry,
    bool deleted,
    bool adminMember,
   DateTime? lastLogin,
    string createdBy,
   DateTime createDate,
   string modifiedBy,
  DateTime? modifiedDate,
   List<Guid> stationIds,
  string email,
   string passwordHash,
   int attachmentsCount,
   List<Guid> tenants,
   List<Guid> landLords,
    Guid ownerKey
        , DateTime timeStamp)
    {
      UserId = userId;
      PartnerId = partnerId;
      Description = description;
      DescriptionTextId = descriptionTextId;
      FirstName = firstName;
      LastName = lastName;
      LastLoginCountry = lastLoginCountry;
      LastLoginCity = lastLoginCity;
      LastLoginState = lastLoginState;
      LastLoginIPAdress = lastLoginIPAdress;
      Adress = adress;
      UserPasswordHash = userPasswordHash;
      Roles = roles;
      LastAttemptLogin = lastAttemptLogin;
      RefreshToken = refreshToken;
      RefreshTokenExpiryTime = refreshTokenExpiryTime;
      UserCountry = userCountry;
      Deleted = deleted;
      AdminMember = adminMember;
      LastLogin = lastLogin;
      CreatedBy = createdBy;
      CreateDate = createDate;
      ModifiedBy = modifiedBy;
      ModifiedDate = modifiedDate;
      StationIds = stationIds;
      Email = email;
      PasswordHash = passwordHash;
      AttachmentsCount = attachmentsCount;
      Tenants = tenants;
      LandLords = landLords;
      OwnerKey = SystemTextId.MemberOwnerId;
      Timestamp = timeStamp;
    }

    [Id(0)]
    public Guid UserId { get; }
    [Id(1)]
    public Guid PartnerId { get; }
    [Id(2)]
    public string Description { get; }
    [Id(3)]
    public Guid DescriptionTextId { get; }
    [Id(4)]
    public string FirstName { get; }
    [Id(5)]
    public string LastName { get; }
    [Id(6)]
    public string LastLoginCountry { get; }
    [Id(7)]
    public string LastLoginCity { get; }
    [Id(8)]
    public string LastLoginState { get; }
    [Id(9)]
    public string LastLoginIPAdress { get; }
    [Id(10)]
    public string Adress { get; }
    [Id(11)]
    public string UserPasswordHash { get; }
    [Id(12)]
    public List<Role> Roles { get; }
    [Id(13)]
    public DateTime LastAttemptLogin { get; }
    [Id(14)]
    public string RefreshToken { get; }
    [Id(15)]
    public DateTime RefreshTokenExpiryTime { get; }
    [Id(16)]
    public string UserCountry { get; }
    [Id(17)]
    public bool Deleted { get; }
    [Id(18)]
    public bool AdminMember { get; }
    [Id(19)]
    public DateTime? LastLogin { get; }
    [Id(20)]
    public string CreatedBy { get; }
    [Id(21)]
    public DateTime CreateDate { get; }
    [Id(22)]
    public string ModifiedBy { get; }
    [Id(23)]
    public DateTime? ModifiedDate { get; }
    [Id(24)]
    public List<Guid> StationIds { get; }
    [Id(25)]
    public string Email { get; }
    [Id(26)]
    public string PasswordHash { get; }
    [Id(27)]
    public Guid OwnerKey { get; }
    [Id(28)]
    public DateTime Timestamp { get; }
    [Id(29)]
    public int AttachmentsCount { get; set; }
    [Id(30)]
    public List<Guid> Tenants { get; set; }
    [Id(31)]
    public List<Guid> LandLords { get; set; }

    public bool Equals(MemberItem memberItem)
    {
      if (memberItem == null) return false;
      return
      Email == memberItem.Email
      && PasswordHash == memberItem.PasswordHash
       && AttachmentsCount == memberItem.AttachmentsCount
     && UserId == memberItem.UserId
     && PartnerId == memberItem.PartnerId
       && Description == memberItem.Description
       && DescriptionTextId == memberItem.DescriptionTextId
       && FirstName == memberItem.FirstName
      && LastName == memberItem.LastName
      && LastLoginCountry == memberItem.LastLoginCountry
      && LastLoginCity == memberItem.LastLoginCity
      && LastLoginState == memberItem.LastLoginState
       && LastLoginIPAdress == memberItem.LastLoginIPAdress
      && Adress == memberItem.Adress
      && UserPasswordHash == memberItem.UserPasswordHash
      && Roles == memberItem.Roles
      && LastAttemptLogin == memberItem.LastAttemptLogin
      && RefreshToken == memberItem.RefreshToken
      && RefreshTokenExpiryTime == memberItem.RefreshTokenExpiryTime
      && UserCountry == memberItem.UserCountry
      && Deleted == memberItem.Deleted
      && AdminMember == memberItem.AdminMember
      && LastLogin == memberItem.LastLogin
      && CreatedBy == memberItem.CreatedBy
      && CreateDate == memberItem.CreateDate
      && ModifiedBy == memberItem.ModifiedBy
      && ModifiedDate == memberItem.ModifiedDate
      && StationIds == memberItem.StationIds
      && OwnerKey == memberItem.OwnerKey
      && Timestamp == memberItem.Timestamp;
    }
  }
}