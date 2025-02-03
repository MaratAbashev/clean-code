using MarkdownWebApi.Application.Assistants;

namespace MarkdownWebApi.Application.Interfaces.Services;

public interface IMarkdownService
{
    Task<Result<string>> ParseText(string text);
}