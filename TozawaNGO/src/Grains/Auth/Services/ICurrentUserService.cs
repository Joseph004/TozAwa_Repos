

using Grains.Auth.Models.Dtos;

namespace Grains.Auth.Services;

public interface ICurrentUserService
{
    CurrentUserDto User { get; set; }
    Guid LanguageId { get; set; }
    bool IsAuthorizedFor(params RoleDto[] functions);
    bool IsAdmin();
}