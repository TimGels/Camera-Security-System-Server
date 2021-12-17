using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace CSS_Server.Utilities
{
    /// <summary>
    /// Helper class for validating and hashing passwords.
    /// </summary>
    public class HashHelper
    {
        private static readonly RandomNumberGenerator _randomNummerGenerator = RandomNumberGenerator.Create();

        //TODO research to hashing. I now used a SO: https://stackoverflow.com/questions/52146528/how-to-validate-salted-and-hashed-password-in-c-sharp
        // I think there are better built in methods in System.Security.Cryptography...

        public static string GenerateHashPbkdf2(string password, out string salt)
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] saltBytes = CreateRandomSalt(128);

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = GetHash(password, saltBytes);

            salt = Convert.ToBase64String(saltBytes);
            return hashed;
        }

        public static bool VerifyPbkdf2(string password, string hashed, string salt)
        {
            return hashed == GetHash(password, Convert.FromBase64String(salt));
        }

        private static string GetHash(string password, byte[] saltBytes)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
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
            byte[] randBytes = length >= 1 ? new byte[length] : new byte[1];

            // Fill the buffer with a cryptographically strong random sequence of values.
            _randomNummerGenerator.GetBytes(randBytes);

            // return the bytes.
            return randBytes;
        }
    }
}
