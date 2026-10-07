using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Errors;

namespace UnitTests.TestImplementation
{
    sealed internal class TestErrorSink : IErrorSink<Span>
    {
        public class ErrorInstance
        {
            public Span Span;
            public ErrorInfo Error;
            public string[] Args;

            public override string ToString() => Error.ToString(Args);
        }

        public readonly List<ErrorInstance> Errors = new List<ErrorInstance>();

        public int MaxCount { get; }

        public TestErrorSink(int maxCount = 1024)
        {
            MaxCount = maxCount;
        }

        public int Count => this.Errors.Count;

        public void Error(Span span, ErrorInfo info, params string[] argsOpt)
        {
            if (this.Errors.Count >= MaxCount)
                throw new InvalidOperationException($"Error sink reached its maximum capacity of {MaxCount} errors.");

            Errors.Add(new ErrorInstance()
            {
                Span = span,
                Error = info,
                Args = argsOpt,
            });
        }
    }
}
