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
            const string input = "_ita li_c";
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
        
        
    }
}