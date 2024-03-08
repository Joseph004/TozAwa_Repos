using Grains.Auth.Models.Authentication;

namespace Grains
{
    [GenerateSerializer]
    [Immutable]
    public class MemberItem : IEquatable<MemberItem>
    {
        public MemberItem(MemberItem memberItem)
            : this(memberItem, DateTime.UtcNow)
        {
        }

        protected MemberItem(MemberItem memberItem, DateTime timeStamp)
        {
            UserId = memberItem.UserId;
            PartnerId = memberItem.PartnerId;
            Description = memberItem.Description;
            DescriptionTextId = memberItem.DescriptionTextId;
            FirstName = memberItem.FirstName;
            LastName = memberItem.LastName;
            LastLoginCountry = memberItem.LastLoginCountry;
            LastLoginCity = memberItem.LastLoginCity;
            LastLoginState = memberItem.LastLoginState;
            LastLoginIPAdress = memberItem.LastLoginIPAdress;
            Adress = memberItem.Adress;
            UserPasswordHash = memberItem.UserPasswordHash;
            Roles = memberItem.Roles;
            LastAttemptLogin = memberItem.LastAttemptLogin;
            RefreshToken = memberItem.RefreshToken;
            RefreshTokenExpiryTime = memberItem.RefreshTokenExpiryTime;
            UserCountry = memberItem.UserCountry;
            Deleted = memberItem.Deleted;
            AdminMember = memberItem.AdminMember;
            LastLogin = memberItem.LastLogin;
            CreatedBy = memberItem.CreatedBy;
            CreateDate = memberItem.CreateDate;
            ModifiedBy = memberItem.ModifiedBy;
            ModifiedDate = memberItem.ModifiedDate;
            StationIds = memberItem.StationIds;
            OwnerKey = memberItem.OwnerKey;
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
        public Guid OwnerKey { get; }
        [Id(26)]
        public DateTime Timestamp { get; }

        public bool Equals(MemberItem memberItem)
        {
            if (memberItem == null) return false;
            return
            UserId == memberItem.UserId
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