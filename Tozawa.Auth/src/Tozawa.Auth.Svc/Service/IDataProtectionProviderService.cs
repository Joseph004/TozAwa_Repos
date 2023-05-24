
namespace Tozawa.Auth.Svc.Services;

public interface IDataProtectionProviderService
{
    Task<byte[]> EncryptAsync(string clearText);
    Task<string> DecryptAsync(byte[] encrypted);
}