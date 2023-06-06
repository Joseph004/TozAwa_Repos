using Blazor.SubtleCrypto;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tozawa.Client.Portal.Configurations;

namespace Tozawa.Client.Portal.Services;

public class DataProtectionProviderService : IDataProtectionProviderService
{
    private readonly AppSettings _appSettings;
    private readonly IJSRuntime _jSRuntime;

    public DataProtectionProviderService(AppSettings appSettings, IJSRuntime jSRuntime)
    {
        _appSettings = appSettings;
        _jSRuntime = jSRuntime;
    }
    private byte[] DeriveKeyFromPassword(string password)
    {
        var emptySalt = Array.Empty<byte>();
        var iterations = 1000;
        var desiredKeyLength = 16; // 16 bytes equal 128 bits.
        var hashMethod = HashAlgorithmName.SHA384;
        return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                         emptySalt,
                                         iterations,
                                         hashMethod,
                                         desiredKeyLength);
    }
    private byte[] IV =
    {
    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };
    public async Task<string> EncryptAsync(string clearText)
    {
        var options = new CryptoOptions() { Key = _appSettings.LoginEncryptKey };
        var crypto = new CryptoService(_jSRuntime, options);
        CryptoResult encrypted = await crypto.EncryptAsync(clearText);
        return encrypted.Value;
    }
  
    public async Task<string> DecryptAsync(string encrypted)
    {
        var options = new CryptoOptions() { Key = _appSettings.LoginEncryptKey };
        var crypto = new CryptoService(_jSRuntime, options);
        string decrypt = await crypto.DecryptAsync(encrypted);

        return decrypt;
    }
}