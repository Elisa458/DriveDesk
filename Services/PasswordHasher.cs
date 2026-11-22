using System.Security.Cryptography;

namespace DriveDesk.Services;

/// <summary>PBKDF2 (SHA-256, 100 000 итераций, соль 16 байт на пользователя).</summary>
public static class PasswordHasher
{
    private const int Iterations = 100_000, SaltSize = 16, HashSize = 32;

    public static (byte[] Hash, byte[] Salt) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return (hash, salt);
    }

    public static bool Verify(string password, byte[] hash, byte[] salt)
    {
        var test = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(test, hash); // защита от timing-атак
    }
}
