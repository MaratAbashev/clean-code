namespace MarkdownWebApi.Core.Models;

public class DocumentModel
{
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; }
    public AccessLevelModel AccessLevel { get; init; }
}

public enum AccessLevelModel
{
    Private,
    OnLink,
    Public
}