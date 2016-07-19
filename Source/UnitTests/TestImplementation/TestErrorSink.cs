using PHP.Core.Text;
using PhpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PHP.Syntax;

namespace UnitTests.TestImplementation
{
    internal class TestErrorSink : IErrorSink<Span>
    {
        public List<Tuple<Span, ErrorInfo, string[]>> Errors = new List<Tuple<Span, ErrorInfo, string[]>>();

        public void Error(Span span, ErrorInfo info, params string[] argsOpt)
        {
            Errors.Add(new Tuple<Span, ErrorInfo, string[]>(span, info, argsOpt));
        }
    }
}
