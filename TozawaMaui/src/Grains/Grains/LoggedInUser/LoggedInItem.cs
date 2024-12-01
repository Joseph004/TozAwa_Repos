using Grains.Helpers;

namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class LoggedInItem : IEquatable<LoggedInItem>
  {
    public LoggedInItem(
    Guid userId,
    string token,
    string refreshToken,
     Guid ownerKey
    )
        : this(
            userId,
   token,
   refreshToken,
   ownerKey,
    DateTime.UtcNow)
    {
    }

    protected LoggedInItem(
        Guid userId,
         string token,
    string refreshToken,
    Guid ownerKey,
    DateTime timeStamp)
    {
      UserId = userId;
      Token = token;
      RefreshToken = refreshToken;
      OwnerKey = SystemTextId.LoggedInOwnerId;
      Timestamp = timeStamp;
    }

    [Id(0)]
    public Guid UserId { get; }
    [Id(1)]
    public string Token { get; }
    [Id(2)]
    public string RefreshToken { get; }
    [Id(3)]
    public Guid OwnerKey { get; }
    [Id(4)]
    public DateTime Timestamp { get; }

    public bool Equals(LoggedInItem loggedInItem)
    {
      if (loggedInItem == null) return false;
      return
     UserId == loggedInItem.UserId
     && Token == loggedInItem.Token
       && RefreshToken == loggedInItem.RefreshToken
         && OwnerKey == loggedInItem.OwnerKey
      && Timestamp == loggedInItem.Timestamp;
    }
  }
}