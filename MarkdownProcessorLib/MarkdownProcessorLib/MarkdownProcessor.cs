using MarkdownProcessorLib.Interfaces;

namespace MarkdownProcessorLib;

public class MarkdownProcessor
{
    private readonly IParser _parser = new Parser();

    public string Process(string input)
    {
        return _parser.ParseToHTML(input);
    }
}