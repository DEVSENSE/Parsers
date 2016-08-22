using System;

namespace Devsense.PHP.Errors
{
    /// <summary>
    /// Provides error sink to report errors into.
    /// </summary>
    /// <typeparam name="TSpan">Type of position.</typeparam>
    public interface IErrorSink<TSpan>
    {
        /// <summary>
        /// Reports an error to the sink.
        /// </summary>
        /// <param name="span">Error position.</param>
        /// <param name="info">Error descriptor.</param>
        /// <param name="argsOpt">Error message arguments.</param>
        void Error(TSpan span, ErrorInfo info, params string[] argsOpt);
    }
}
