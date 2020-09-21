using System;
using System.Security.Cryptography;
using System.Text;

namespace CCMS.Server.Utilities
{
    public class HashUtil
    {
        public static string SaltAndHashPassword(string password, string salt)
        {
            using var sha = SHA256.Create();
            var saltedPassword = password + salt;
            var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword));
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string hashedPassword, string password, string salt)
        {
            return hashedPassword == SaltAndHashPassword(password, salt);
        }

        public static string GenerateRandomSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
