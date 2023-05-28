
using System.Threading.Tasks;

namespace TozAwaHome.Services;

public interface IDataProtectionProviderService
{
    Task<byte[]> EncryptAsync(string clearText);
    Task<string> DecryptAsync(byte[] encrypted);
}