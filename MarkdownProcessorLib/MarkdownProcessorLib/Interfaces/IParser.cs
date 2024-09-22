using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownProcessorLib.Interfaces
{
    public interface IParser
    {
        public string ParseToHTML(string input);
    }
}
