namespace MarkdownWebApp.DataAccess.Postgres.Models;

public class UserEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<DocumentAccess>? UserDocuments { get; set; } = new();
}