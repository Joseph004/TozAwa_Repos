
using System.Threading.Tasks;

namespace Tozawa.Language.Client.Services;

public interface IDataProtectionProviderService
{
    Task<byte[]> EncryptAsync(string clearText);
    Task<string> DecryptAsync(byte[] encrypted);
}