
namespace Grains.Models.Organization.Store
{
    [GenerateSerializer]
    public class OrganizationStates
    {
        public HashSet<Guid> Items { get; set; }
    }

    [GenerateSerializer]
    public class OrganizationState
    {
        public OrganizationItem Organization { get; set; }
    }
}