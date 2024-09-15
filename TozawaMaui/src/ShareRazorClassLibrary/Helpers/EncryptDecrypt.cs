using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using ShareRazorClassLibrary.Configurations;

namespace TozawaNGO;

public interface IEncryptDecrypt
{
    string EncryptUsingCertificate(string data);
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
            string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var assembly = solutiondir + "\\src\\" + "ShareRazorClassLibrary";
            string path = Path.Combine(assembly, @"Helpers/mycert.pem");
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
}