using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax
{
    class LexerState : ILexerState
    {
        #region ILexerState members

        /// <inheritDoc/>
        public int CurrentState { get; set; }

        /// <inheritDoc/>
        public CompleteToken CurrentToken => Lookahead(0);

        /// <inheritDoc/>
        public CompleteToken PreviousToken => _previousToken;

        /// <inheritDoc/>
        public ICollection<int> ExpectedTokens => _expectedTokens.Keys;

        /// <inheritDoc/>
        public bool IsExpectedToken(int token) => _expectedTokens.ContainsKey(token);

        /// <inheritDoc/>
        public CompleteToken Lookahead(ushort index)
        {
            BufferTokens(index);
            return _buffer[index];
        }

        /// <inheritDoc/>
        public void SetToken(ushort index, CompleteToken token)
        {
            BufferTokens(index);
            _buffer.Insert(index, token);
        }

        /// <inheritDoc/>
        public void RemoveToken(ushort index)
        {
            BufferTokens(index);
            _buffer.RemoveAt(index);
        }

        #endregion

        List<CompleteToken> _buffer;
        Dictionary<int, int> _expectedTokens;
        ITokenProvider<SemanticValueType, Span> _provider;
        CompleteToken _previousToken;

        /// <summary>
        /// Tokens buffer managed by the state. 
        /// It is used to reset the lexer after error recovery.
        /// </summary>
        public List<CompleteToken> TokensBuffer => _buffer;

        /// <summary>
        /// Construct a new parser state based on its current state.
        /// </summary>
        /// <param name="state">Current parser state.</param>
        /// <param name="expectedTokens">Expected tokens.</param>
        /// <param name="previousToken">Previous token.</param>
        /// <param name="currentToken">Current token.</param>
        /// <param name="provider">Currently used lexer.</param>
        public LexerState(int state, Dictionary<int, int> expectedTokens, CompleteToken previousToken, CompleteToken currentToken, ITokenProvider<SemanticValueType, Span> provider)
        {
            CurrentState = state;
            _buffer = new List<CompleteToken>() { currentToken };
            _expectedTokens = expectedTokens;
            _provider = provider;
            _previousToken = previousToken;
        }

        private void BufferTokens(uint index)
        {
            for (int i = _buffer.Count; i <= index; i++)
            {
                var lookahead = new CompleteToken((Tokens)_provider.GetNextToken(), _provider.TokenValue, _provider.TokenPosition, _provider.TokenText);
                _buffer.Add(lookahead);
            }
        }
    }
}
