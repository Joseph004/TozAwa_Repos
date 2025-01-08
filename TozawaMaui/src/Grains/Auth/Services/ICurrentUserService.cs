

using Grains.Auth.Models.Dtos;
using Grains.Models;

namespace Grains.Auth.Services;

public interface ICurrentUserService
{
    CurrentUserDto User { get; set; }
    Guid LanguageId { get; set; }
    bool IsAuthorizedFor(params FunctionType[] functions);
    bool IsAdmin();
}