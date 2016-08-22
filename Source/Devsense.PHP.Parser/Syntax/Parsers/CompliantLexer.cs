using System;

using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// PHP lexer generated according to the PhpLexer.l grammar.
    /// Special implementation of the PHP lexer that skips empty nodes (open tag, comment, etc.), which is required by the PHP grammar.
    /// </summary>
    internal class CompliantLexer : ITokenProvider<SemanticValueType, Span>
    {
        ITokenProvider<SemanticValueType, Span> _provider;

        /// <summary>
        /// Lexer constructor that initializes all the necessary members
        /// </summary>
        /// <param name="provider">Underlaying tokens provider.</param>
        public CompliantLexer(ITokenProvider<SemanticValueType, Span> provider)
        {
            _provider = provider;
        }

        public PHPDocBlock DocBlock { get { return _provider.DocBlock; } set { _provider.DocBlock = value; } }

        public Span TokenPosition => _provider.TokenPosition;

        public string TokenText => _provider.TokenText;

        public SemanticValueType TokenValue => _provider.TokenValue;

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

                // origianl zendlex() functionality - skip open and close tags because they are not in the PHP grammar
                switch (token)
                {
                    case Tokens.T_COMMENT:
                    case Tokens.T_DOC_COMMENT:
                    case Tokens.T_OPEN_TAG:
                    case Tokens.T_WHITESPACE:
                        continue;
                    case Tokens.T_CLOSE_TAG:
                        token = Tokens.T_SEMI; /* implicit ; */
                        break;
                    case Tokens.T_OPEN_TAG_WITH_ECHO:
                        token = Tokens.T_ECHO;
                        break;
                }
                
                return (int)token;
            } while (true);
        }

        public void ReportError(string[] expectedTokens)
        {
            _provider.ReportError(expectedTokens);
        }
    }
}
