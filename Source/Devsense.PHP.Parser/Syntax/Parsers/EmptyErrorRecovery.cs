using Devsense.PHP.Errors;
using System.Collections.Generic;
using System;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Empty implementation of <see cref="IErrorRecovery"/>.
    /// </summary>
    public sealed class EmptyErrorRecovery : IErrorRecovery
    {
        public static IErrorRecovery Instance = new EmptyErrorRecovery();

        private EmptyErrorRecovery() { }

        /// <summary>
        /// Always fails to recover.
        /// </summary>
        /// <param name="lexerState">Current parser state.</param>
        /// <returns>Always <c>False</c>.</returns>
        public bool TryRecover(ILexerState lexerState) => false;
    }
}
