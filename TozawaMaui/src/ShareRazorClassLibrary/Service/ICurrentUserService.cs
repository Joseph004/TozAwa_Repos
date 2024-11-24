using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Services;

public interface ICurrentUserService
{
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasRole(string role);
}