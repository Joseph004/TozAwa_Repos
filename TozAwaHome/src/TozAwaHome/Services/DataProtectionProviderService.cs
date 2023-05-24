using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TozAwaHome.Configurations;

namespace TozAwaHome.Services;

public class DataProtectionProviderService : IDataProtectionProviderService
{
    private readonly AppSettings _appSettings;

    public DataProtectionProviderService(AppSettings appSettings)
    {
        _appSettings = appSettings;
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
    public async Task<byte[]> EncryptAsync(string clearText)
    {
        var firstOutPut = await Crypt(clearText, _appSettings.LoginEncryptKey);

        var secondOutPut = await CryptBytes(firstOutPut);

        return secondOutPut;
    }
    private async Task<byte[]> Crypt(string text, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(key);
        aes.IV = IV;
        using MemoryStream output = new();
        using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(text));
        await cryptoStream.FlushFinalBlockAsync();
        return output.ToArray();
    }

    private async Task<byte[]> CryptBytes(byte[] passBytes)
    {
        using Aes aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(_appSettings.AADClient.TenantId);
        aes.IV = IV;
        using MemoryStream output = new();
        using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await cryptoStream.WriteAsync(passBytes);
        await cryptoStream.FlushFinalBlockAsync();
        return output.ToArray();
    }
    private async Task<byte[]> DecryptByte(byte[] encrypted)
    {
        using Aes aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(_appSettings.AADClient.TenantId);
        aes.IV = IV;
        using MemoryStream input = new(encrypted);
        using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using MemoryStream output = new();
        await cryptoStream.CopyToAsync(output);
        return output.ToArray();
    }
    private async Task<string> Decrypt(byte[] encrypted, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(key);
        aes.IV = IV;
        using MemoryStream input = new(encrypted);
        using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using MemoryStream output = new();
        await cryptoStream.CopyToAsync(output);
        return Encoding.Unicode.GetString(output.ToArray());
    }
    public async Task<string> DecryptAsync(byte[] encrypted)
    {
        var firstOutPut = await DecryptByte(encrypted);

        var secondOutPut = await Decrypt(firstOutPut, _appSettings.LoginEncryptKey);

        return secondOutPut;
    }
}