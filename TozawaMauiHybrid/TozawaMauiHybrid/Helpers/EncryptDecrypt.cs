using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TozawaMauiHybrid.Helpers;

public interface IEncryptDecrypt
{
    string EncryptUsingCertificate(string data);
}
public class EncryptDecrypt() : IEncryptDecrypt
{
    public string EncryptUsingCertificate(string data)
    {
        try
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            var assembly = AppDomain.CurrentDomain.BaseDirectory;
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