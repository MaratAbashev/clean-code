using System.Security.Cryptography;
using MarkdownWebApi.Application;
using MarkdownWebApi.Application.Interfaces.Auth;

namespace MarkdownWebApi.Infrastructure;

public class PasswordHashier: IPasswordHashier
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA256;
    private const char SaltDelimeter = ';';

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName, KeySize);
        return string.Join(SaltDelimeter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public bool VerifyHashedPassword(string passwordHash, string password)
    {
        var pwdElements = passwordHash.Split(SaltDelimeter);
        var salt = Convert.FromBase64String(pwdElements[0]);
        var hash = Convert.FromBase64String(pwdElements[1]);
        var hashInput = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName, KeySize);
        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}