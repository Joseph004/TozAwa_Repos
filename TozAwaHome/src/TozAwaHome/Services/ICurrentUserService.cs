

using System.Collections.Generic;
using System.Threading.Tasks;
using TozAwaHome.Models.Dtos;

namespace TozAwaHome.Services;

public interface ICurrentUserService
{
    Task RemoveCurrentUser();
    Task SetCurrentUser(CurrentUserDto user);
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasAtLeastOneFeature(List<int> features);
    Task<bool> HasFeature(int feature);
    Task<bool> HasFunctionType(string functionType);
}