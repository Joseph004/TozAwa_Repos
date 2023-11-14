

using TozawaNGO.Auth.Models.Dtos;

namespace TozawaNGO.Auth.Services;

public interface ICurrentUserService
{
    CurrentUserDto User { get; set; }

    bool IsAuthorizedFor(params RoleDto[] functions);
    bool IsAdmin();
}