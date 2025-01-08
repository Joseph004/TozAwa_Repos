
namespace Grains.Auth.Models.Dtos
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Role Role { get; set; }
        public List<FunctionDto> Functions { get; set; }
    }
    public enum Role
    {
        None = 0,
        President = 1,
        VicePresident = 2,
        Cashier = 3,
        Signatory = 4,
        LandLoard = 5,
        Tenant = 6,
        Economist = 7,
        CareTaker = 8 //vakmästare
    }
}
