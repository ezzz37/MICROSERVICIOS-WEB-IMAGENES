using System;
using System.Security.Cryptography;

namespace AuthService.Helpers
{
    public static class EncryptionHelper
    {
        // Cuando registres usuarios, usa esto:
        public static byte[] HashPassword(string plainTextPassword)
        {
            // Genera un salt de 16 bytes
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            // Deriva una key de 32 bytes
            using var pbkdf2 = new Rfc2898DeriveBytes(plainTextPassword, salt, 100_000, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(32);

            // Prepara array final: [ salt (16) | key (32) ]
            var hashed = new byte[16 + 32];
            Buffer.BlockCopy(salt, 0, hashed, 0, 16);
            Buffer.BlockCopy(key, 0, hashed, 16, 32);
            return hashed;
        }

        // Para el login, compara el plain con el hash almacenado:
        public static bool VerifyPassword(string plainTextPassword, byte[] hashedPasswordWithSalt)
        {
            // Extrae salt
            var salt = new byte[16];
            Buffer.BlockCopy(hashedPasswordWithSalt, 0, salt, 0, 16);

            // Genera key con el mismo salt
            using var pbkdf2 = new Rfc2898DeriveBytes(plainTextPassword, salt, 100_000, HashAlgorithmName.SHA256);
            var keyToCheck = pbkdf2.GetBytes(32);

            // Extrae la key original
            var originalKey = new byte[32];
            Buffer.BlockCopy(hashedPasswordWithSalt, 16, originalKey, 0, 32);

            // Compara byte a byte
            return CryptographicOperations.FixedTimeEquals(keyToCheck, originalKey);
        }
    }
}
