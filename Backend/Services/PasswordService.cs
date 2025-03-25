using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Backend.Services
{
    public class PasswordService
    {
        // Hashes a password and returns a string containing both the hash and salt
        public string HashPassword(string password)
        {
            // Generate a 16-byte salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password using PBKDF2
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            );

            // Combine salt + hash into one string (Base64 encoded)
            byte[] combinedHash = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, combinedHash, 0, salt.Length);
            Array.Copy(hash, 0, combinedHash, salt.Length, hash.Length);

            return Convert.ToBase64String(combinedHash);
        }

        // Verifies a password against a stored hash
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Convert the stored hash from Base64 to byte array
            byte[] combinedHash = Convert.FromBase64String(storedHash);

            // Extract the salt (first 16 bytes)
            byte[] salt = new byte[16];
            Array.Copy(combinedHash, 0, salt, 0, salt.Length);

            // Extract the stored hash (remaining bytes)
            byte[] originalHash = new byte[combinedHash.Length - salt.Length];
            Array.Copy(combinedHash, salt.Length, originalHash, 0, originalHash.Length);

            // Hash the entered password with the same salt
            byte[] enteredHash = KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            );

            // Compare both hashes securely
            return CryptographicOperations.FixedTimeEquals(originalHash, enteredHash);
        }
    }
}
