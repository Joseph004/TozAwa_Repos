using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ShareRazorClassLibrary.Services
{
    public interface IPasswordHashService
    {
        string HashPasword(string password, out byte[] salt);
        bool VerifyPassword(string password, string hash, byte[] salt);
    }
    public class PasswordHashService : IPasswordHashService
    {
        const int keySize = 64;
        const int iterations = 350000;
        readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

        public PasswordHashService()
        {

        }
        public string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                _hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }
        public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, _hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
