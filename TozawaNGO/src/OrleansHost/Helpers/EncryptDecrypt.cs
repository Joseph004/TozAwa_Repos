using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using OrleansHost.Configurations;

namespace OrleansHost;

public interface IEncryptDecrypt
{
    string EncryptUsingCertificate(string data);
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

    public string EncryptUsingCertificate(string data)
    {
        try
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Helpers/mycert.pem");
            //Path.Combine(_hostEnvironment.WebRootPath, "Helpers/mycert.pem");
            var collection = new X509Certificate2Collection();
            collection.Import(path);
            var certificate = collection[0];
            var output = "";
            #pragma warning disable SYSLIB0027
            using (RSA csp = (RSA)certificate.PublicKey.Key)
            {
                byte[] bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.OaepSHA1);
                output = Convert.ToBase64String(bytesEncrypted);
            }
            return output;
        }
        catch (Exception)
        {
            return "";
        }
    }

    public string DecryptUsingCertificate(string data)
    {
        try
        {
            byte[] byteData = Convert.FromBase64String(data);
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Helpers/mycertprivatekey.pfx");
            //Path.Combine(_hostEnvironment.WebRootPath, "Helpers/mycertprivatekey.pfx");
            var Password = _appSettings.PrivateKey; //Note This Password is That Password That We Have Put On Generate Keys  
            var collection = new X509Certificate2Collection();
            collection.Import(System.IO.File.ReadAllBytes(path), Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            #pragma warning disable SYSLIB0026
            X509Certificate2 certificate = new();
            certificate = collection[0];
            foreach (var cert in collection)
            {
                if (cert.FriendlyName.Contains("my certificate"))
                {
                    certificate = cert;
                }
            }
            if (certificate.HasPrivateKey)
            {
                #pragma warning disable SYSLIB0028
                RSA csp = (RSA)certificate.PrivateKey;
                var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;
                var keys = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.OaepSHA1));
                return keys;
            }
        }
        catch (Exception) { }
        return null;
    }
}