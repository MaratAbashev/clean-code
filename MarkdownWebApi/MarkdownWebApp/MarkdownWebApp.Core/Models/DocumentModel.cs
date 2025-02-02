namespace MarkdownWebApi.Core.Models;

public class DocumentModel
{
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; }
    public string AccessLevel { get; init; }
}