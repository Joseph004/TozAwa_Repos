using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Services;

public interface ICurrentUserService
{
    Task RemoveCurrentUser();
    Task SetCurrentUser(CurrentUserDto user);
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasRole(string role);
}