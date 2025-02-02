namespace MarkdownWebApi.Core.Models;

public class DocumentAccessModel
{
    public Guid UserId { get; init; }
    public Guid DocumentId { get; init; }
    public string? DocumentName { get; init; }
    public RoleModel Role { get; init; }
    public List<UserModel>? Users { get; init; }
}

public enum RoleModel
{
    Creator,
    Editor,
    Watcher
}