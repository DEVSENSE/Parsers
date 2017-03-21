using System;
using System.Collections.Generic;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Lexer state that can be modified by an implementation of the <see cref="Errors.IErrorRecovery"/>.
    /// The state is created from the lexers current state and modified by the error recovery.
    /// It is then used to change the lexer state befor the parser continues.
    /// </summary>
    public interface ILexerState
    {
        /// <summary>
        /// Current parser state (terminal/non-terminal).
        /// </summary>
        int CurrentState { get; set; }

        /// <summary>
        /// List of all tokens allowed affter the current one.
        /// </summary>
        ICollection<int> ExpectedTokens { get; }

        /// <summary>
        /// Checks if the token is expected.
        /// </summary>
        /// <param name="token">Terget token.</param>
        /// <returns><c>True</c> if token is expected, <c>False</c> otherwise.</returns>
        bool IsExpectedToken(int token);

        /// <summary>
        /// Current token that caused the error.
        /// </summary>
        CompleteToken CurrentToken { get; }

        /// <summary>
        /// Last token before the current one.
        /// </summary>
        CompleteToken PreviousToken { get; }

        /// <summary>
        /// Get a future token, indexed from the current one (<c>0</c>) to the end.
        /// Method is able to return tokens beyond the end of the code (returns <see cref="Tokens.END"/> in that case).
        /// </summary>
        /// <param name="index">Index of the future token (<c>0</c> for current token).</param>
        /// <returns>The future token.</returns>
        CompleteToken Lookahead(ushort index);

        /// <summary>
        /// Set a new future token. 
        /// The tokens with the index equal or greater than the new one are moved by one space.
        /// </summary>
        /// <param name="index">Index of the new token (<c>0</c> for current token).</param>
        /// <param name="token">The new token.</param>
        void SetToken(ushort index, CompleteToken token);

        /// <summary>
        /// Remove a future token.
        /// </summary>
        /// <param name="index">Index of the removed token (<c>0</c> for current token).</param>
        void RemoveToken(ushort index);
    }
}
