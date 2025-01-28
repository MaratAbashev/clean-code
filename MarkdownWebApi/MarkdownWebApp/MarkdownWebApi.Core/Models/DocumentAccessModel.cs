namespace MarkdownWebApi.Core.Models;

public class DocumentAccessModel
{
    public Guid UserId { get; init; }
    public string DocumentName { get; init; }
    public string Role { get; init; }
}