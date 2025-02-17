using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Grains.Configurations;

namespace Grains.Helpers;

public interface IEncryptDecrypt
{
    string DecryptUsingCertificate(string data);
}
public class EncryptDecrypt : IEncryptDecrypt
{
    private static IWebHostEnvironment _hostEnvironment;
    private readonly AppSettings _appSettings;
    public EncryptDecrypt(IWebHostEnvironment environment, AppSettings appSettings)
    {
        _hostEnvironment = environment;
        _appSettings = appSettings;
    }
    public string DecryptUsingCertificate(string data)
    {
        try
        {
            byte[] byteData = Convert.FromBase64String(data);
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Helpers/mycertprivatekey.pfx");
            var Password = _appSettings.PrivateKey; //Note This Password is That Password That We Have Put On Generate Keys  
            var collection = new X509Certificate2Collection();
            collection.Import(System.IO.File.ReadAllBytes(path), Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            var certificate = collection[0];
            foreach (var cert in collection)
            {
                if (cert.FriendlyName.Contains("my certificate"))
                {
                    certificate = cert;
                }
            }
            if (certificate.HasPrivateKey)
            {
                RSA csp = (RSA)certificate.GetRSAPrivateKey();
                var privateKey = certificate.GetRSAPrivateKey() as RSACryptoServiceProvider;
                var keys = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.OaepSHA1));
                return keys;
            }
        }
        catch (Exception) { }
        return null;
    }
}