using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Text;
using System.Diagnostics;

namespace Devsense.PHP.Syntax
{
    // TODO: remove or document
    internal sealed class BufferedLexer : IParserTokenProvider<SemanticValueType, Span>
    {
        private CompliantLexer _provider;
        private Stack<CompleteToken> _buffer = new Stack<CompleteToken>();
        int _currentToken = (int)Tokens.END;
        CompleteToken _previousToken = CompleteToken.Empty;

        public BufferedLexer(CompliantLexer provider)
        {
            _provider = provider;
        }

        #region IParserTokenProvider members

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

        public DocCommentList DocBlockList => _provider.DocBlockList;

        public void ReportError(string[] expectedTokens) => _provider.ReportError(expectedTokens);

        #endregion

        private CompleteToken BackupToken() => new CompleteToken((Tokens)_currentToken, TokenValue, TokenPosition, TokenText);

        public CompleteToken PreviousToken => _previousToken;

        public void AddNextToken(CompleteToken token, CompleteToken previous)
        {
            _buffer.Push(token);
            _buffer.Push(_previousToken = previous);
        }

        public void AddNextTokens(IList<CompleteToken> tokensBuffer, CompleteToken previousToken)
        {
            foreach (var item in tokensBuffer.Reverse())
            {
                _buffer.Push(item);
            }
            _buffer.Push(_previousToken = previousToken);
        }
    }
}
