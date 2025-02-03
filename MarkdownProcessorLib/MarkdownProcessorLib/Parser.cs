using System.Text;
using MarkdownProcessorLib.Interfaces;

namespace MarkdownProcessorLib;

public class Parser : IParser
{
    public string ParseToHTML(string input)
{
    if (string.IsNullOrEmpty(input))
        return string.Empty;
    var sanitizedInput = SanitizeInput(input);
    var lines = SplitInputIntoLines(sanitizedInput);
    return BuildHTMLDocument(lines);
}

private string SanitizeInput(string input)
{
    return input
        .Replace("<", "&lt;")
        .Replace(">", "&gt;");
}

private string[] SplitInputIntoLines(string input)
{
    return input.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
}

private string BuildHTMLDocument(string[] lines)
{
    var result = new StringBuilder();
    
    foreach (var line in lines)
    {
        result.AppendLine(ProcessLine(line));
    }
    
    return result.ToString().Trim();
}

private string ProcessLine(string line)
{
    if (IsEmptyLine(line))
    {
        return "<br>";
    }

    return IsHeaderLine(line) 
        ? ProcessHeaderLine(line) 
        : ProcessParagraphLine(line);
}

private bool IsEmptyLine(string line)
{
    return string.IsNullOrWhiteSpace(line);
}

private bool IsHeaderLine(string line)
{
    return line.StartsWith("#") 
           && CountHeaderLevel(line) <= 6 
           && line.Skip(CountHeaderLevel(line)).FirstOrDefault() == ' '
           && char.IsLetter(line.Skip(CountHeaderLevel(line) + 1).FirstOrDefault());
}

private int CountHeaderLevel(string line)
{
    return line.TakeWhile(c => c == '#').Count();
}

private string ProcessHeaderLine(string line)
{
    var level = CountHeaderLevel(line);
    var content = line.Substring(level).Trim();
    return $"<h{level}>{ParseInlineFormatting(content)}</h{level}>";
}

private string ProcessParagraphLine(string line)
{
    return $"<p>{ParseInlineFormatting(line.Trim())}</p>";
}

private string ParseInlineFormatting(string input)
{
    var result = new StringBuilder();
    int position = 0;
    
    while (position < input.Length)
    {
        var (processedLength, output) = ProcessNextToken(input, position);
        result.Append(output);
        position += processedLength;
    }
    
    return result.ToString().Trim();
}

private (int processedLength, string output) ProcessNextToken(string input, int position)
{
    if (TryProcessEscapeSequence(input, position, out var escapeResult))
        return escapeResult;
    
    if (TryProcessBoldFormatting(input, position, out var boldResult))
        return boldResult;
    
    if (TryProcessItalicFormatting(input, position, out var italicResult))
        return italicResult;
    
    return (1, input[position].ToString());
}

private bool TryProcessEscapeSequence(string input, int position, out (int, string) result)
{
    result = default;
    if (position + 1 >= input.Length) return false;
    
    if (input[position] == '\\' && IsEscapeableCharacter(input[position + 1]))
    {
        result = (2, input[position + 1].ToString());
        return true;
    }
    return false;
}

private bool IsEscapeableCharacter(char c) => c == '_' || c == '\\';

private bool TryProcessBoldFormatting(string input, int position, out (int, string) result)
{
    result = default;
    if (position + 1 >= input.Length || input[position] != '_' || input[position + 1] != '_')
        return false;

    var endPosition = FindFormattingEnd(input, position + 2, "__");
    if (endPosition == -1)
    {
        result = (1, "_");
        return true;
    }

    var content = input.Substring(position + 2, endPosition - position - 2);
    result = (endPosition - position + 2, $"<b>{content}</b>");
    return true;
}

private bool TryProcessItalicFormatting(string input, int position, out (int, string) result)
{
    result = default;
    if (input[position] != '_') return false;

    var endPosition = FindFormattingEnd(input, position + 1, "_");
    if (endPosition == -1)
    {
        result = (1, "_");
        return true;
    }

    var content = input.Substring(position + 1, endPosition - position - 1);
    result = (endPosition - position + 1, $"<i>{content}</i>");
    return true;
}

private int FindFormattingEnd(string input, int startPosition, string marker)
{
    var endPosition = input.IndexOf(marker, startPosition);
    return endPosition != -1 && !char.IsWhiteSpace(input[endPosition - 1])
        ? endPosition
        : -1;
}
}