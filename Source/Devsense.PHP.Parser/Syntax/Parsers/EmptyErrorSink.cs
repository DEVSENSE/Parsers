using Devsense.PHP.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Empty implementation of error sink.
    /// </summary>
    internal class EmptyErrorSink<TSpan> : IErrorSink<TSpan>
    {
        public void Error(TSpan span, ErrorInfo info, params string[] argsOpt) { }
    }
}
