using Devsense.PHP.Errors;
using System.Collections.Generic;
using System;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Empty implementation of <see cref="IErrorRecovery"/>.
    /// </summary>
    public class EmptyErrorRecovery : IErrorRecovery
    {
        /// <summary>
        /// Always fails to recover.
        /// </summary>
        /// <param name="lexerState">Current parser state.</param>
        /// <returns>Always <c>False</c>.</returns>
        public bool TryRecover(ILexerState lexerState) => false;
    }
}
