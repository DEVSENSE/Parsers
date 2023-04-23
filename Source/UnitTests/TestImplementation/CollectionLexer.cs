using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Errors;
using Devsense.PHP.Syntax;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTests.TestImplementation
{
    internal class CollectionLexer : ITokenProvider<SemanticValueType, Span>
    {
        List<Comment> _comments = new List<Comment>();
        ITokenProvider<SemanticValueType, Span> _provider;
        public List<Comment> Comments => _comments;
        List<ISourceToken> _tokens = new List<ISourceToken>();

        /// <summary>
        /// Lexer constructor that initializes all the necessary members
        /// </summary>
        /// <param name="provider">Underlaying tokens provider.</param>
        public CollectionLexer(StringReader source, IErrorSink<Span> errors)
        {
            // TODO: should have CompliantLexer a LanguageFeatures !!!

            _provider = new Lexer(source, Encoding.UTF8, errors,
                features: LanguageFeatures.Basic,
                initialState: Lexer.LexicalStates.INITIAL);
        }

        public Span TokenPosition => _provider.TokenPosition;

        public string TokenText => _provider.TokenText;

        public ReadOnlySpan<char> TokenTextSpan => _provider.TokenTextSpan;

        public SemanticValueType TokenValue
        {
            get => _provider.TokenValue;
            set => _provider.TokenValue = value;
        }

        public List<ISourceToken> AllTokens => _tokens;

        /// <summary>
        /// Get next token and store its actual position in the source unit.
        /// This implementation supports the functionality of zendlex, which skips empty nodes (open tag, comment, etc.).
        /// </summary>
        /// <returns>Next token.</returns>
        public int GetNextToken()
        {
            do
            {
                Tokens token = (Tokens)_provider.GetNextToken();
                _tokens.Add(new SourceToken(token, TokenPosition));

                // origianl zendlex() functionality - skip open and close tags because they are not in the PHP grammar
                switch (token)
                {
                    case Tokens.T_DOC_COMMENT:
                    case Tokens.T_COMMENT:
                    case Tokens.T_OPEN_TAG:
                    case Tokens.T_CLOSE_TAG:
                        _comments.Add(new Comment(TokenPosition, TokenText));
                        break;
                    case Tokens.T_WHITESPACE:
                    case Tokens.T_OPEN_TAG_WITH_ECHO:
                        break;
                }

                return (int)token;
            } while (true);
        }

        public void ReportError(string[] expectedTokens)
        {
            _provider.ReportError(expectedTokens);
        }

        public CompleteToken PreviousToken => CompleteToken.Empty;

        public IDocBlock DocComment { get => null; set => throw new NotImplementedException(); }

        public void AddNextTokens(IList<CompleteToken> tokensBuffer, CompleteToken previousToken) { }
    }
}
