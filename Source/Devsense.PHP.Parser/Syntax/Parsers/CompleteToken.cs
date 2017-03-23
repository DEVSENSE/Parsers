
using Devsense.PHP.Text;
using System.Diagnostics;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Compact representation of a lexer token including its data and text.
    /// </summary>
    [DebuggerDisplay("{Token} - {TokenText} - {TokenPosition}")]
    public struct CompleteToken
    {
        /// <summary>
        /// Token.
        /// </summary>
        public readonly Tokens Token;

        /// <summary>
        /// Token data.
        /// </summary>
        public readonly SemanticValueType TokenValue;

        /// <summary>
        /// Token span.
        /// </summary>
        public readonly Span TokenPosition;

        /// <summary>
        /// Token text.
        /// </summary>
        public readonly string TokenText;

        /// <summary>
        /// Empty token singleton.
        /// </summary>
        public static readonly CompleteToken Empty = new CompleteToken(Tokens.T_ERROR, new SemanticValueType(), Span.Invalid, string.Empty);

        /// <summary>
        /// Create a token.
        /// </summary>
        /// <param name="token">Token.</param>
        /// <param name="tokenValue">Token data.</param>
        /// <param name="tokenPosition">Token span.</param>
        /// <param name="tokenText">Token text.</param>
        public CompleteToken(Tokens token, SemanticValueType tokenValue, Span tokenPosition, string tokenText)
        {
            Token = token;
            TokenValue = tokenValue;
            TokenPosition = tokenPosition;
            TokenText = tokenText;
        }
    }
}
