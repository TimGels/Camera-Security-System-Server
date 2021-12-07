using System;
using System.Security.Cryptography;

namespace CSS_Server.Utilities
{
    /// <summary>
    /// Helper class for validating and hashing passwords.
    /// </summary>
    public class HashHelper
    {

        //TODO research to hashing. I now used a SO: https://stackoverflow.com/questions/52146528/how-to-validate-salted-and-hashed-password-in-c-sharp
        // I think there are better built in methods in System.Security.Cryptography...

        public static string GenerateHash(string password, out string salt)
        {
            byte[] saltBytes = CreateRandomSalt(64);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);

            salt = Convert.ToBase64String(saltBytes);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
        }

        public static bool Verify(string password, string hash, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == hash;
        }

        //////////////////////////////////////////////////////////
        // Helper methods:
        // CreateRandomSalt: Generates a random salt value of the
        //                   specified length.
        //
        // source: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.passwordderivebytes?view=net-6.0
        //////////////////////////////////////////////////////////

        private static byte[] CreateRandomSalt(int length)
        {
            // Create a buffer
            byte[] randBytes;

            if (length >= 1)
            {
                randBytes = new byte[length];
            }
            else
            {
                randBytes = new byte[1];
            }

            // Create a new RNGCryptoServiceProvider.
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

            // Fill the buffer with random bytes.
            rand.GetBytes(randBytes);

            // return the bytes.
            return randBytes;
        }
    }
}
