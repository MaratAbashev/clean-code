using System.Text;
using MarkdownProcessorLib.Interfaces;

namespace MarkdownProcessorLib;

public class Parser : IParser
{
    public string ParseToHTML(string input)
    {
        input = input.Replace("<", "&lt;").Replace(">", "&gt;");
        var textSplittedByLines = input.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        foreach (var textSplittedByLine in textSplittedByLines)
            Console.WriteLine(textSplittedByLine);
        var result = new StringBuilder();

        foreach (var line in textSplittedByLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                result.AppendLine("<br>");
                continue;
            }

            if (line.StartsWith("#") && line.TakeWhile(symbol => symbol == '#').Count() <= 6)
            {
                var level = line.TakeWhile(symbol => symbol == '#').Count();
                result.AppendLine($"<h{level}>{ParseInlineFormatting(line.Substring(level).Trim())}</h{level}>");
            }
            else
            {
                result.AppendLine("<p>" + ParseInlineFormatting(line.Trim()) + "</p>");
            }
        }

        return result.ToString().Trim();
    }

    private string ParseInlineFormatting(string input)
    {
        var result = new StringBuilder();
        var i = 0;
        while (i < input.Length)
        {
            if (i + 1 < input.Length && input[i] == '\\' && (input[i + 1] == '_' || input[i + 1] == '\\'))
            {
                result.Append(input[i + 1]);
                i += 2;
            }
            else if (i + 1 < input.Length && input[i] == '_' && input[i + 1] == '_' &&
                     (i == 0 || !char.IsWhiteSpace(input[i - 1])))
            {
                var endIndex = input.IndexOf("__", i + 2);
                if (endIndex > i + 1 && !char.IsWhiteSpace(input[endIndex - 1]))
                {
                    result.Append("<b>").Append(input.Substring(i + 2, endIndex - i - 2)).Append("</b>");
                    i = endIndex + 1;
                }
                else
                {
                    result.Append("_");
                }
            }
            else if (input[i] == '_' && (i == 0 || !char.IsWhiteSpace(input[i - 1])))
            {
                var endIndex = input.IndexOf('_', i + 1);
                if (endIndex > i && !char.IsWhiteSpace(input[endIndex - 1]))
                {
                    result.Append("<i>").Append(input.Substring(i + 1, endIndex - i - 1)).Append("</i>");
                    i = endIndex;
                }
                else
                {
                    result.Append("_");
                }
            }
            else
            {
                result.Append(input[i]);
            }

            i++;
        }

        return result.ToString().Trim();
    }
}