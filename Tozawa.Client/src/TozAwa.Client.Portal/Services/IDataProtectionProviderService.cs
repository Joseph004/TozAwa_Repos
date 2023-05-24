
using System.Threading.Tasks;

namespace Tozawa.Client.Portal.Services;

public interface IDataProtectionProviderService
{
    Task<byte[]> EncryptAsync(string clearText);
    Task<string> DecryptAsync(byte[] encrypted);
}