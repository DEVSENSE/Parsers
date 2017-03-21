using System.Collections.Generic;
using Devsense.PHP.Syntax;

namespace Devsense.PHP.Errors
{
    /// <summary>
    /// Interface for error recovery used by the <see cref="Parser"/>.
    /// </summary>
    public interface IErrorRecovery
    {
        /// <summary>
        /// Try to recover from the current syntax error (unexpected token).
        /// The error recovery is provided with the actual lexer and parsers state, which can be modified.
        /// The recovery can modify the state and reprot whether the error was repaired or not.
        /// </summary>
        /// <param name="lexerState">Current lexer state.</param>
        /// <returns><c>True</c> if the error was repaired, <c>False</c> otherwise.</returns>
        bool TryRecover(ILexerState lexerState);
    }
}
