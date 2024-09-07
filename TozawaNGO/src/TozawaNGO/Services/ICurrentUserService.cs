using TozawaNGO.Models.Dtos;

namespace TozawaNGO.Services;

public interface ICurrentUserService
{
    Task RemoveCurrentUser();
    Task SetCurrentUser(CurrentUserDto user);
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasRole(string role);
}