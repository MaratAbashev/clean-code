namespace MarkdownWebApi.Core.Models;

public class UserModel
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string PasswordHash { get; init; }
}