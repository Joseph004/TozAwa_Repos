using Grains.Auth.Models.Authentication;
using Grains.Models;

namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class MemberItem : IEquatable<MemberItem>
  {
    public MemberItem(
    Guid userId,
    string description,
    Guid descriptionTextId,
     string firstName,
     string lastName,
   string lastLoginCountry,
   string lastLoginCity,
    string lastLoginState,
  string lastLoginIPAdress,
    List<RoleEnum> roles,
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
    List<int> features,
    List<FunctionType> functions,
    string comment,
    Guid commentTextId,
   List<Guid> organizationIds,
   string cityCode,
   string countryCode,
   Gender gender,
    Guid ownerKey
    )
        : this(
            userId,
    description,
    descriptionTextId,
     firstName,
     lastName,
   lastLoginCountry,
   lastLoginCity,
    lastLoginState,
  lastLoginIPAdress,
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
   features,
   functions,
   comment,
   commentTextId,
  organizationIds,
   cityCode,
   countryCode,
   gender,
    ownerKey,
    DateTime.UtcNow)
    {
    }

    protected MemberItem(
        Guid userId,
    string description,
    Guid descriptionTextId,
     string firstName,
     string lastName,
   string lastLoginCountry,
   string lastLoginCity,
    string lastLoginState,
  string lastLoginIPAdress,
    List<RoleEnum> roles,
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
   List<int> features,
   List<FunctionType> functions,
   string comment,
   Guid commentTextId,
   List<Guid> organizationIds,
   string cityCode,
   string countryCode,
   Gender gender,
    Guid ownerKey
        , DateTime timeStamp)
    {
      UserId = userId;
      Description = description;
      DescriptionTextId = descriptionTextId;
      FirstName = firstName;
      LastName = lastName;
      LastLoginCountry = lastLoginCountry;
      LastLoginCity = lastLoginCity;
      LastLoginState = lastLoginState;
      LastLoginIPAdress = lastLoginIPAdress;
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
      Features = features;
      Functions = functions;
      OwnerKey = ownerKey;
      Comment = comment;
      CommentTextId = commentTextId;
      OrganizationIds = organizationIds;
      CityCode = cityCode;
      CountryCode = countryCode;
      Gender = gender;
      Timestamp = timeStamp;
    }

    [Id(0)]
    public Guid UserId { get; }
    [Id(1)]
    public string Description { get; }
    [Id(2)]
    public Guid DescriptionTextId { get; }
    [Id(3)]
    public string FirstName { get; }
    [Id(4)]
    public string LastName { get; }
    [Id(5)]
    public string LastLoginCountry { get; }
    [Id(6)]
    public string LastLoginCity { get; }
    [Id(7)]
    public string LastLoginState { get; }
    [Id(8)]
    public string LastLoginIPAdress { get; }
    [Id(9)]
    public List<RoleEnum> Roles { get; }
    [Id(10)]
    public DateTime LastAttemptLogin { get; }
    [Id(11)]
    public string RefreshToken { get; }
    [Id(12)]
    public DateTime RefreshTokenExpiryTime { get; }
    [Id(13)]
    public string UserCountry { get; }
    [Id(14)]
    public bool Deleted { get; }
    [Id(15)]
    public bool AdminMember { get; }
    [Id(16)]
    public DateTime? LastLogin { get; }
    [Id(17)]
    public string CreatedBy { get; }
    [Id(18)]
    public DateTime CreateDate { get; }
    [Id(19)]
    public string ModifiedBy { get; }
    [Id(20)]
    public DateTime? ModifiedDate { get; }
    [Id(21)]
    public List<Guid> StationIds { get; }
    [Id(22)]
    public string Email { get; }
    [Id(23)]
    public string PasswordHash { get; }
    [Id(24)]
    public Guid OwnerKey { get; }
    [Id(25)]
    public DateTime Timestamp { get; }
    [Id(26)]
    public int AttachmentsCount { get; set; }
    [Id(27)]
    public List<Guid> Tenants { get; set; }
    [Id(28)]
    public List<Guid> LandLords { get; set; }
    [Id(29)]
    public List<int> Features { get; set; }
    [Id(30)]
    public List<FunctionType> Functions { get; set; }
    [Id(31)]
    public string Comment { get; }
    [Id(32)]
    public Guid CommentTextId { get; }
    [Id(33)]
    public List<Guid> OrganizationIds { get; }
    [Id(34)]
    public string CityCode { get; }
    [Id(35)]
    public string CountryCode { get; }
    [Id(36)]
    public Gender Gender { get; set; }
    public bool Equals(MemberItem memberItem)
    {
      if (memberItem == null) return false;
      return
      Email == memberItem.Email
      && PasswordHash == memberItem.PasswordHash
       && AttachmentsCount == memberItem.AttachmentsCount
       && Tenants == memberItem.Tenants
       && LandLords == memberItem.LandLords
       && Features == memberItem.Features
       && Functions == memberItem.Functions
     && UserId == memberItem.UserId
      && OrganizationIds == memberItem.OrganizationIds
       && Description == memberItem.Description
       && DescriptionTextId == memberItem.DescriptionTextId
       && FirstName == memberItem.FirstName
      && LastName == memberItem.LastName
      && LastLoginCountry == memberItem.LastLoginCountry
      && LastLoginCity == memberItem.LastLoginCity
      && LastLoginState == memberItem.LastLoginState
       && LastLoginIPAdress == memberItem.LastLoginIPAdress
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
      && Comment == memberItem.Comment
      && CommentTextId == memberItem.CommentTextId
      && CityCode == memberItem.CityCode
      && CountryCode == memberItem.CountryCode
      && Timestamp == memberItem.Timestamp;
    }
  }
}