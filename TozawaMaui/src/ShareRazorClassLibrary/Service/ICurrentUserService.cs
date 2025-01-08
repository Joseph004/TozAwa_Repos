using ShareRazorClassLibrary.Models.Dtos;

namespace ShareRazorClassLibrary.Services;

public interface ICurrentUserService
{
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasAtLeastOneFeature(List<int> features);
    Task<bool> HasFeature(int feature);
    Task<bool> HasFunctionType(string functionType);
}