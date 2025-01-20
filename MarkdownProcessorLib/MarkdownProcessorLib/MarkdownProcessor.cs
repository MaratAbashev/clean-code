using MarkdownProcessorLib.Interfaces;

namespace MarkdownProcessorLib;

public class MarkdownProcessor
{
    public IHTMLManager _manager;
    public IParser _parser;

    public MarkdownProcessor()
    {
        _manager = new HTMLManager();
        _parser = new Parser();
    }

    public void Process(string input)
    {
        _manager.CreateHTML(_parser.ParseToHTML(input));
    }
}