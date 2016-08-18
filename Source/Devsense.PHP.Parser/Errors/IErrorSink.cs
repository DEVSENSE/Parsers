using System;

namespace Devsense.PHP.Errors
{
    public interface IErrorSink<TSpan>
    {
        /// <summary>
        /// Report error. TODO
        /// </summary>
        /// <param name="span">Entire element span.</param>
        /// <param name="info">Error type.</param>
        /// <param name="argsOpt">Additional error informatin</param>
        void Error(TSpan span, ErrorInfo info, params string[] argsOpt);
    }
}
