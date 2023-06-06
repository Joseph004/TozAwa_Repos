
using System.Threading.Tasks;

namespace Tozawa.Client.Portal.Services;

public interface IDataProtectionProviderService
{
    Task<string> EncryptAsync(string clearText);
    Task<string> DecryptAsync(string encrypted);
}