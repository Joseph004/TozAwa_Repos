using TozawaMauiHybrid.Models.Dtos;

namespace TozawaMauiHybrid.Services;

public interface ICurrentUserService
{
    void RemoveCurrentUser();
    void SetCurrentUser(CurrentUserDto user);
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasRole(string role);
}