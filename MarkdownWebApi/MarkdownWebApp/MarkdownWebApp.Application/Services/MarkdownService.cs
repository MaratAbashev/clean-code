using MarkdownProcessorLib;
using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Services;

namespace MarkdownWebApi.Application.Services;

public class MarkdownService : IMarkdownService
{
    private readonly MarkdownProcessor _markdownProcessor = new MarkdownProcessor();

    public async Task<Result<string>> ParseText(string text)
    {
        var parsedText = await Task.Run(() => _markdownProcessor.Process(text));
        return Result<string>.Ok(parsedText);
    }
}