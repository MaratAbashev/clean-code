namespace MarkdownWebApp.DataAccess.Postgres.Models;

public class DocumentEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Private;
    public List<DocumentAccess>? UserDocuments { get; set; } = new();
}

public enum AccessLevel
{
    Private,
    OnLink,
    Public
}