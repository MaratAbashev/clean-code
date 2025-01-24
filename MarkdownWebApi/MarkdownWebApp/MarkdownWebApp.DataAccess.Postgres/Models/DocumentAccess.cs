namespace MarkdownWebApp.DataAccess.Postgres.Models;

public class DocumentAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public Guid DocumentId { get; set; }
    public DocumentEntity? Document { get; set; }
    public Role Role { get; set; } = Role.Creator;
}

public enum Role
{
    Creator,
    Editor,
    Watcher
}