

using Tozawa.Auth.Svc.Models.Dtos;
using Tozawa.Auth.Svc.Models.Enums;

namespace Tozawa.Auth.Svc.Services;

public interface ICurrentUserService
{
    CurrentUserDto User { get; set; }

    bool IsAuthorizedFor(params FunctionType[] functions);
    bool IsRoot();

    bool HasFeatures(IEnumerable<int> features);
    bool HasFeatures(params int[] features);
}