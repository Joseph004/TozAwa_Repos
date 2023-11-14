
namespace TozawaNGO.Auth.Services;

public interface IDataProtectionProviderService
{
    string EncryptString(string key, string plainText);
    string DecryptString(string key, string cipherText);
}