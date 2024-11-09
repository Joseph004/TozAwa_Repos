using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TozawaMauiHybrid.Helpers;

public static class Cryptography
{
    public static string EncryptUsingCertificate(string data, byte[] fileBytes)
    {
        var output = "";
        try
        {
            var assembly = AppDomain.CurrentDomain.BaseDirectory;
            string extension = "pem";
            string path = Path.Combine(assembly, @"mycert") + "." + extension;
            File.WriteAllBytes(path, fileBytes);
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            var collection = new X509Certificate2Collection();

            collection.Import(path);
            var certificate = collection[0];
            using (RSA csp = (RSA)certificate.GetRSAPublicKey())
            {
                byte[] bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.OaepSHA1);
                output = Convert.ToBase64String(bytesEncrypted);
            }
            File.Delete(path);
            return output;
        }
        catch (Exception)
        {
            output = "";
            throw new ArgumentException(null, nameof(data));
        }
    }
    public static byte[] Encrypt(byte[] plainBytes, string key, string ivStr)
    {
        var iv = Encoding.UTF8.GetBytes(ivStr);
        byte[] keyBytes = null;
        using (SHA256 mySHA256 = SHA256.Create())
        {
            var valueArr = Encoding.UTF8.GetBytes(key);
            using MemoryStream ms = new(valueArr);
            //compute hash
            keyBytes = mySHA256.ComputeHash(ms);
        }
        byte[] encryptedBytes = null;

        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Encrypt the input plaintext using the AES algorithm
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        return encryptedBytes;
    }

    public static byte[] Decrypt(byte[] cipherBytes, string key, string ivStr)
    {
        var iv = Encoding.UTF8.GetBytes(ivStr);
        byte[] keyBytes = null;
        using (SHA256 mySHA256 = SHA256.Create())
        {
            var valueArr = Encoding.UTF8.GetBytes(key);
            using MemoryStream ms = new(valueArr);
            //compute hash
            keyBytes = mySHA256.ComputeHash(ms);
        }
        byte[] decryptedBytes = null;

        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Decrypt the input ciphertext using the AES algorithm
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        }

        return decryptedBytes;
    }

    public static byte[] Encrypt(string plainText, string key, string ivStr)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var iv = Encoding.UTF8.GetBytes(ivStr);
        byte[] keyBytes = null;
        using (SHA256 mySHA256 = SHA256.Create())
        {
            var valueArr = Encoding.UTF8.GetBytes(key);
            using MemoryStream ms = new(valueArr);
            //compute hash
            keyBytes = mySHA256.ComputeHash(ms);
        }
        byte[] encryptedBytes = null;

        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Encrypt the input plaintext using the AES algorithm
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        return encryptedBytes;
    }
    public static byte[] Encrypt(byte[] plainBytes, string key, byte[] iv)
    {
        byte[] keyBytes = null;
        using (SHA256 mySHA256 = SHA256.Create())
        {
            var valueArr = Encoding.UTF8.GetBytes(key);
            using MemoryStream ms = new(valueArr);
            //compute hash
            keyBytes = mySHA256.ComputeHash(ms);
        }
        byte[] encryptedBytes = null;

        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Encrypt the input plaintext using the AES algorithm
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        return encryptedBytes;
    }

    public static byte[] Decrypt(byte[] cipherBytes, string key, byte[] iv)
    {
        byte[] keyBytes = null;
        using (SHA256 mySHA256 = SHA256.Create())
        {
            var valueArr = Encoding.UTF8.GetBytes(key);
            using MemoryStream ms = new(valueArr);
            //compute hash
            keyBytes = mySHA256.ComputeHash(ms);
        }
        byte[] decryptedBytes = null;

        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Decrypt the input ciphertext using the AES algorithm
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        }

        return decryptedBytes;
    }

    public static byte[] Encrypt(string plainText, string key, byte[] iv)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = null;
        using (SHA256 mySHA256 = SHA256.Create())
        {
            var valueArr = Encoding.UTF8.GetBytes(key);
            using MemoryStream ms = new(valueArr);
            //compute hash
            keyBytes = mySHA256.ComputeHash(ms);
        }
        byte[] encryptedBytes = null;

        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Encrypt the input plaintext using the AES algorithm
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        return encryptedBytes;
    }
}