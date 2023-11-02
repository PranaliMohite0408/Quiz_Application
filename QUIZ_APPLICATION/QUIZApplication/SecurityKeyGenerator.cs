using System;
using System.Security.Cryptography;

public static class SecurityKeyGenerator
{
    public static string GenerateSecurityKey()
    {
        // Generate a new security key
        byte[] key = new byte[32]; // 256 bits
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }

        // Convert the key to a base64 string for storage
        return Convert.ToBase64String(key);
    }
}
