using Devsense.PHP.Errors;
using System.Collections.Generic;
using Devsense.PHP.Syntax;
using System.Linq;
using Devsense.PHP.Text;

namespace UnitTests.TestImplementation
{
    /// <summary>
    /// Basic implementation of <see cref="IErrorRecovery"/> for unit tests.
    /// </summary>
    class TestErrorRecovery : IErrorRecovery 
    {
        /// <summary>
        /// Basic error recovery that adds a samicolon if it is possible.
        /// </summary>
        /// <param name="lexerState">Current parser state.</param>
        /// <returns><c>True</c> is semicolon is expected, <c>False</c> otherwise.</returns>
        public bool TryRecover(ILexerState lexerState)
        {
            if (lexerState.IsExpectedToken((int)Tokens.T_SEMI))
            {
                lexerState.SetToken(0, new CompleteToken(Tokens.T_SEMI, new SemanticValueType(), new Span(lexerState.PreviousToken.TokenPosition.End, 0), ";"));
                return true;
            }
            return false;
        }
    }
}
