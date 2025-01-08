using Grains.Auth.Models.Authentication;
using Grains.Models;

namespace Grains
{
  [GenerateSerializer]
  [Immutable]
  public class RoleItem : IEquatable<RoleItem>
  {
    public RoleItem(
  Guid id,
   Guid organizationId,
   RoleEnum role,
   List<FunctionType> functions,
   Guid ownerKey
    )
        : this(
             id,
    organizationId,
    role,
    functions,
    ownerKey,
    DateTime.UtcNow)
    {
    }

    protected RoleItem(
        Guid id,
   Guid organizationId,
   RoleEnum role,
   List<FunctionType> functions,
   Guid ownerKey
        , DateTime timeStamp)
    {
      Id = id;
      OrganizationId = organizationId;
      Role = role;
      Functions = functions;
      OwnerKey = ownerKey;
      Timestamp = timeStamp;
    }

    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public Guid OrganizationId { get; set; }
    [Id(2)]
    public RoleEnum Role { get; set; }
    [Id(3)]
    public List<FunctionType> Functions { get; set; }
    [Id(4)]
    public Guid OwnerKey { get; }
    [Id(5)]
    public DateTime Timestamp { get; }

    public bool Equals(RoleItem RoleItem)
    {
      if (RoleItem == null) return false;
      return
      Id == RoleItem.Id
      && OrganizationId == RoleItem.OrganizationId
       && Role == RoleItem.Role
       && Functions == RoleItem.Functions
       && OwnerKey == RoleItem.OwnerKey
       && Timestamp == RoleItem.Timestamp;
    }
  }
}