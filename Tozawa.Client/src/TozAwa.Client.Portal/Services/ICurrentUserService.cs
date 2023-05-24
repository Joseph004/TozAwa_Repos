

using System.Collections.Generic;
using System.Threading.Tasks;
using Tozawa.Client.Portal.Models.Dtos;

namespace Tozawa.Client.Portal.Services;

public interface ICurrentUserService
{
    Task RemoveCurrentUser();
    Task SetCurrentUser(CurrentUserDto user);
    Task<CurrentUserDto> GetCurrentUser();
    Task<bool> HasAtLeastOneFeature(List<int> features);
    Task<bool> HasFeature(int feature);
    Task<bool> HasFunctionType(string functionType);
}