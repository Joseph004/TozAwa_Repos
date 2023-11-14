
namespace TozawaNGO.Auth.Models.Dtos;

public class CurrentUserDto
{
    public string UserName { get; init; } = "";
    public string Email { get; init; } = "";
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Adress { get; set; } = "";
    public bool Admin { get; init; }
    public List<RoleDto> Roles { get; init; } = new();
    public Guid PartnerId { get; init; }
    public string Partner { get; init; } = "";
    public string Country { get; set; } = "";
    public Guid Id { get; init; }
}