
using System.Security.Claims;
using Tozawa.Language.Client.Models.Dtos;

namespace Tozawa.Language.Client.Services;

public interface ICurrentUserService
{
    Task RemoveCurrentUser();
    Task<ClaimsPrincipal> GetAuthenticationStateAsync();
    Task SetCurrentUser(CurrentUserDto user);
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasAtLeastOneFeature(List<int> features);
    Task<bool> HasFeature(int feature);
    Task<bool> HasFunctionType(string functionType);
}