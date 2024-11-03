using System;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace NGO.Business
{
	public class PasswordHandler
	{
        private const int SaltSize = 16; // 16 bytes for salt
        private const int HashSize = 32; // 32 bytes for hash
        private const int Iterations = 4; // Recommended for Argon2id

        // Generate a password hash using Argon2id
        public string HashPassword(string password)
        {
            var salt = GenerateSalt();
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8, // Number of threads to use
                Iterations = Iterations,
                MemorySize = 8192 // Memory in KB, 8 MB recommended for Argon2
            };

            var hashBytes = argon2.GetBytes(HashSize);
            var hash = Convert.ToBase64String(hashBytes);
            var saltBase64 = Convert.ToBase64String(salt);

            return $"{saltBase64}:{hash}";
        }

        // Verify a password against the stored hash
        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                throw new FormatException("Unexpected hash format");

            var salt = Convert.FromBase64String(parts[0]);
            var storedPasswordHash = parts[1];

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = Iterations,
                MemorySize = 8192
            };

            var hashBytes = argon2.GetBytes(HashSize);
            var hash = Convert.ToBase64String(hashBytes);

            return hash == storedPasswordHash;
        }

        // Generate a random salt for each password
        private byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}

