namespace MarkdownWebApi.Application.Interfaces.Auth;

public interface IPasswordHashier
{
    public string HashPassword(string password);
    public bool VerifyHashedPassword(string hashedPassword, string password);
}