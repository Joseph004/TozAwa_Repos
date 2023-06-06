
namespace Tozawa.Auth.Svc.Services;

public interface IDataProtectionProviderService
{
    Task<string> EncryptAsync(string clearText);
    Task<string> DecryptAsync(string encrypted);
}