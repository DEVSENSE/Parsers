using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.TestImplementation
{
    class WhitespaceComposer : ITokenComposer
    {
        public StringBuilder Code { get { ProcessWhitespaces(new Span(int.MaxValue, 0)); return _builder; } }
        private StringBuilder _builder = new StringBuilder();

        public List<ISourceToken> Processed => _processed;
        private List<ISourceToken> _processed = new List<ISourceToken>();

        private Span _previous = Span.Invalid;
        private ISourceTokenProvider _tokens;

        public WhitespaceComposer(ISourceTokenProvider tokens)
        {
            _tokens = tokens;
        }

        private void ProcessToken(Tokens token, string text, Span position, LangElement sourceNode)
        {
            var start = position.StartOrInvalid;
            var end = start + text.Length;
            if (_builder.Length <= end)
            {
                _builder.Append(' ', end - _builder.Length);
            }
            if (start >= 0 && text.Length >= 0)
            {
                _builder.Replace(start, text.Length, text);
                _processed.Add(new SourceToken(token, position));
            }
        }

        public void ConsumeToken(Tokens token, string text, Span position, LangElement sourceNode)
        {
            ProcessWhitespaces(position);
            if (token != Tokens.T_SEMI || _tokens.GetTokenAt(position, Tokens.T_SEMI, null) != null) // TODO - last element without semicolon
            {
                ProcessToken(token, _tokens.GetTokenText(new SourceToken(token, position), text), position, sourceNode);
            }
        }

        private void ProcessWhitespaces(Span position)
        {
            if (position.IsValid && (!_previous.IsValid || _previous.End <= position.Start))
            {
                var whitespaceSpan = Span.FromBounds(_previous.IsValid ? _previous.End : 0, position.Start);
                var tokens = _tokens.GetTokens(whitespaceSpan, t => t.Token == Tokens.T_WHITESPACE || t.Token == Tokens.T_COMMENT ||
                t.Token == Tokens.T_DOC_COMMENT || t.Token == Tokens.T_OPEN_TAG || t.Token == Tokens.T_CLOSE_TAG, System.Linq.Enumerable.Empty<ISourceToken>());
                if (tokens != null)
                {
                    foreach (var item in tokens)
                    {
                        ProcessToken(item.Token, _tokens.GetTokenText(item, string.Empty), item.Span, null);
                    }
                }
                _previous = position;
            }
        }
    }
}
