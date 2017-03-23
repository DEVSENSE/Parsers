using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    internal class BufferedLexer : ITokenProvider<SemanticValueType, Span>
    {
        private CompliantLexer _provider;
        private Stack<CompleteToken> _buffer = new Stack<CompleteToken>();
        int _currentToken;
        CompleteToken _previousToken;

        public BufferedLexer(CompliantLexer provider)
        {
            _provider = provider;
        }

        #region ITokenProvider members

        public int GetNextToken()
        {
            if (_buffer.Count > 1)
            {
                _previousToken = _buffer.Peek();
                _buffer.Pop();
                Debug.Assert(_buffer.Count > 0);
                return _currentToken = (int)_buffer.Peek().Token;
            }
            else
            {
                _previousToken = BackupToken();
                _buffer.Clear();
                return _currentToken = _provider.GetNextToken();
            }
        }

        public SemanticValueType TokenValue => _buffer.Count > 0 ? _buffer.Peek().TokenValue : _provider.TokenValue;

        public Span TokenPosition => _buffer.Count > 0 ? _buffer.Peek().TokenPosition : _provider.TokenPosition;

        public string TokenText => _buffer.Count > 0 ? _buffer.Peek().TokenText : _provider.TokenText;

        public PHPDocBlock DocBlock
        {
            get
            {
                return _provider.DocBlock;
            }
            set
            {
                _provider.DocBlock = value;
            }
        }

        public void ReportError(string[] expectedTokens) => _provider.ReportError(expectedTokens);

        #endregion

        #region CompliantLexer members

        public DocCommentList DocBlockList => _provider.DocBlockList;

        #endregion

        private CompleteToken BackupToken() => new CompleteToken((Tokens)_currentToken, TokenValue, TokenPosition, TokenText);

        public CompleteToken PreviousToken => _previousToken;

        public void AddNextToken(CompleteToken token, CompleteToken previous)
        {
            _buffer.Push(token);
            _buffer.Push(_previousToken = previous);
        }

        public void AddNextTokens(IList<CompleteToken> tokens, CompleteToken previous)
        {
            foreach (var item in tokens.Reverse())
            {
                _buffer.Push(item);
            }
            _buffer.Push(_previousToken = previous);
        }
    }
}
