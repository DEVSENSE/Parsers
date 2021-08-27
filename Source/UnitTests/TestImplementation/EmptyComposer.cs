using Devsense.PHP.Syntax;
using Devsense.PHP.Text;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.TestImplementation
{
    class EmptyComposer : ITokenComposer
    {
        public StringBuilder Code { get { /*ProcessWhitespaces(Tokens.T_INLINE_HTML, new Span(int.MaxValue, 0));*/ return _builder; } }
        private StringBuilder _builder = new StringBuilder();

        public List<ISourceToken> Processed => _processed;
        private List<ISourceToken> _processed = new List<ISourceToken>();

        private Tokens _previousToken = Tokens.END;
        private Span _previous = Span.Invalid;
        ISourceTokenProvider _tokens;

        public EmptyComposer(ISourceTokenProvider tokens)
        {
            _tokens = tokens;
        }

        private void ProcessToken(Tokens token, string text, Span position)
        {
            var start = position.Start;
            var end = start + text.Length;
            //if (start >= 0 && text.Length >= 0)
            {
                _builder.Append(text);
                _processed.Add(new SourceToken(token, position));
            }
        }

        public void ConsumeToken(Tokens token, string text, Span position)
        {
            ProcessWhitespaces(token, position);
            ProcessToken(token, _tokens.GetTokenText(new SourceToken(token, position), text), position);
            _previousToken = token;
            _previous = position;
        }

        private void ProcessWhitespaces(Tokens token, Span position)
        {
            if ((_previousToken == Tokens.END || _previousToken == Tokens.T_INLINE_HTML) && token != Tokens.T_INLINE_HTML)
            {
                ProcessToken(_previousToken = Tokens.T_OPEN_TAG, "<?php", _previous.IsValid ? _previous = new Span(_previous.End, 5) : Span.Invalid);
                ProcessToken(_previousToken = Tokens.T_WHITESPACE, " ", _previous.IsValid ? _previous = new Span(_previous.End, 1) : Span.Invalid);
            }
            else if (_previousToken != Tokens.END && _previousToken != Tokens.T_INLINE_HTML && token == Tokens.T_INLINE_HTML)
            {
                ProcessToken(_previousToken = Tokens.T_CLOSE_TAG, "?>", _previous.IsValid ? _previous = new Span(_previous.End, 2) : Span.Invalid);
            }
            else if (_previousToken != Tokens.END && _previousToken != Tokens.T_INLINE_HTML && token != Tokens.T_END_HEREDOC)
            {
                ProcessToken(Tokens.T_WHITESPACE, _previousToken == Tokens.T_END_HEREDOC ? "\n" : " ", _previous.IsValid ? _previous = new Span(_previous.End, 1) : Span.Invalid);
                _previousToken = Tokens.T_WHITESPACE;
            }
        }
    }
}
