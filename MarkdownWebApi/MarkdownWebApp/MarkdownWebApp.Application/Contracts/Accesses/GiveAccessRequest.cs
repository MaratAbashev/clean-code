namespace MarkdownWebApi.Application.Contracts.Accesses;

public record GiveAccessRequest(string Email, Guid DocumentId);