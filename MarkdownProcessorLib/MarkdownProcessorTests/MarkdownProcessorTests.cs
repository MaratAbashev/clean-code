using MarkdownProcessorLib;
using MarkdownProcessorLib.Interfaces;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace MarkdownProcessorTests
{
    public class MarkdownProcessorShould
    {
        private IParser _parser = new Parser();

        [Fact]
        public void GiveItalicString()
        {
            const string input = "_italic_";
            const string expected = "<p><i>italic</i></p>";
            string output = _parser.ParseToHTML(input);
            
            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void GiveBoldString()
        {
            const string input = "__bold__";
            const string expected = "<p><b>bold</b></p>";
            string output = _parser.ParseToHTML(input);
            
            Assert.Equal(expected, output);
        }
        
        [Theory]
        [InlineData("# header", "<h1>header</h1>")]
        [InlineData("## header", "<h2>header</h2>")]
        [InlineData("### header", "<h3>header</h3>")]
        [InlineData("#### header", "<h4>header</h4>")]
        [InlineData("##### header", "<h5>header</h5>")]
        [InlineData("###### header", "<h6>header</h6>")]
        public void GiveHeaders(string header, string expected)
        {
            string output = _parser.ParseToHTML(header);
            
            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void SeparateParagraphs()
        {
            const string input = "first paragraph \n\nsecond paragraph";
            const string expected = "<p>first paragraph</p>\r\n<br>\r\n<p>second paragraph</p>";
            string output = _parser.ParseToHTML(input);
            
            Assert.Equal(expected, output);
        }
        
        [Theory]
    [InlineData("_italic_", "<p><i>italic</i></p>")]
    [InlineData("__bold__", "<p><b>bold</b></p>")]
    [InlineData("_italic_with_underscore_", "<p><i>italic_with_underscore</i></p>")]
    public void FormattingTests(string input, string expected)
    {
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NestedFormatting()
    {
        var input = "__bold _and italic_ text__";
        var expected = "<p><b>bold <i>and italic</i> text</b></p>";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapeCharacters()
    {
        var input = @"\_escape_ and \\";
        var expected = "<p>_escape_ and \\</p>";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("####### invalid header", "<p>####### invalid header</p>")]
    [InlineData("#", "<p>#</p>")]
    public void InvalidHeaders(string input, string expected)
    {
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MixedContent()
    {
        var input = "# Header _with_ __formatting__\n\nParagraph __with__ _formatting_";
        var expected = "<h1>Header <i>with</i> <b>formatting</b></h1>\r\n<br>\r\n<p>Paragraph <b>with</b> <i>formatting</i></p>";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HtmlEscaping()
    {
        var input = "<script>alert()</script>";
        var expected = "<p>&lt;script&gt;alert()&lt;/script&gt;</p>";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EmptyInput()
    {
        var input = "";
        var expected = "";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void WhitespaceHandling()
    {
        var input = "   _trimmed_   ";
        var expected = "<p><i>trimmed</i></p>";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MultipleNewLines()
    {
        var input = "Line1\n\n\n\nLine2";
        var expected = "<p>Line1</p>\r\n<br>\r\n<br>\r\n<br>\r\n<p>Line2</p>";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EdgeCaseFormatting()
    {
        var input = "_unclosed format";
        var expected = "<p>_unclosed format</p>";
        var result = _parser.ParseToHTML(input);
        Assert.Equal(expected, result);
    }
    }
}