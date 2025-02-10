using System.Security.Cryptography;
using System.Text;

namespace OrleansHost.Auth.Controllers;

public static class Cryptography
{
    public static byte[] Encrypt(byte[] plainBytes, string key, string ivStr)
    {
        if (plainBytes == null || plainBytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(plainBytes));
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (string.IsNullOrEmpty(ivStr))
        {
            throw new ArgumentNullException(nameof(ivStr));
        }

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
        if (cipherBytes == null || cipherBytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(cipherBytes));
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (string.IsNullOrEmpty(ivStr))
        {
            throw new ArgumentNullException(nameof(ivStr));
        }


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
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentNullException(nameof(plainText));
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (string.IsNullOrEmpty(ivStr))
        {
            throw new ArgumentNullException(nameof(ivStr));
        }

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
        if (plainBytes == null || plainBytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(plainBytes));
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (iv == null || iv.Length <= 0)
        {
            throw new ArgumentNullException(nameof(iv));
        }
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
        if (cipherBytes == null || cipherBytes.Length <= 0)
        {
            throw new ArgumentNullException(nameof(cipherBytes));
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (iv == null || iv.Length <= 0)
        {
            throw new ArgumentNullException(nameof(iv));
        }

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
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentNullException(nameof(plainText));
        }
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (iv == null || iv.Length <= 0)
        {
            throw new ArgumentNullException(nameof(iv));
        }

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