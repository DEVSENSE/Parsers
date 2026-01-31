using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Re-interpret token based on given language level.
    /// Re-interprets to <c>T_STRING</c> when inside a qualified name.
    /// </summary>
    public class ReinterpretingLexer<TProvider, TTokenData> : ITokenProvider<SemanticValueType, Span>
        where TProvider: ITokenProvider<SemanticValueType, Span>
        where TTokenData : struct
    {
        #region TokenSnapshot

        /// <summary>
        /// Current token information. used for lookahead and lookback.
        /// </summary>
        public readonly struct TokenSnapshot
        {
            public readonly Tokens Token;
            public readonly Span TokenPosition;
            public readonly ReadOnlyMemory<char> TokenSource;
            public readonly SemanticValueType TokenValue;

            /// <summary>
            /// Custom data associated with token.
            /// Always resolved.
            /// </summary>
            public readonly TTokenData Data;

            public TokenSnapshot(Tokens token, TTokenData data, TProvider lexer)
            {
                this.Token = token;
                this.TokenPosition = lexer.TokenPosition;
                this.TokenSource = lexer.TokenSource;
                this.TokenValue = lexer.TokenValue;
                this.Data = data;
            }
        }

        #endregion

        #region ITokenProvider

        public IDocBlock DocComment => _provider.DocComment;

        public Span TokenPosition => _current_token.TokenPosition;

        public string TokenText => TokenSource.ToString();

        public ReadOnlySpan<char> TokenTextSpan => TokenSource.Span;

        public ReadOnlyMemory<char> TokenSource => _current_token.TokenSource;

        public SemanticValueType TokenValue => _current_token.TokenValue;

        #endregion

        protected readonly TProvider _provider;

        readonly LanguageFeatures _features;

        /// <param name="provider">Underlying tokens provider.</param>
        /// <param name="language">Language features.</param>
        public ReinterpretingLexer(TProvider provider, LanguageFeatures language = LanguageFeatures.Basic)
        {
            _provider = provider;
            _features = language;
        }

        TokenSnapshot? _previous_token, _lookahead_token;
        TokenSnapshot _current_token;

        bool HasFeatureSet(LanguageFeatures fset) => (_features & fset) == fset;

        /// <summary>
        /// Gets <typeparamref name="TTokenData"/> value associated with current token.
        /// </summary>
        protected virtual TTokenData ResolveTokenData() => default(TTokenData);

        /// <summary>
        /// Advances <see cref="_current_token"/> to the next token.
        /// </summary>
        /// <returns>Value indicating there was another token (i.e. it was not <c>EOF</c>).</returns>
        public bool MoveNext()
        {
            _previous_token = _current_token;

            if (_lookahead_token.HasValue) // already fetched
            {
                _current_token = _lookahead_token.GetValueOrDefault();
                _lookahead_token = default(TokenSnapshot?);
            }
            else
            {
                _current_token = FetchToken(_provider);
            }

            //
            return _current_token.Token != Tokens.END;
        }

        public TokenSnapshot Current => _current_token;

        /// <summary>
        /// Gets the previous token snapshot or default(<see cref="TokenSnapshot"/>) if enumerator has not been moved from first position yet.
        /// </summary>
        public TokenSnapshot Previous => _previous_token.GetValueOrDefault();

        Tokens CurrentToken => _current_token.Token;

        Tokens PreviousToken => Previous.Token;

        /// <summary>
        /// Gets token AFTER the the current one.
        /// Current token must be fetched, and underlying lexer must be in valid state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Underlying lexer is not in valid state.</exception>
        protected TokenSnapshot GetLookaheadToken()
        {
            Debug.Assert(_current_token.Token != 0);

            if (_current_token.Token == 0)
            {
                throw new InvalidOperationException();
            }

            if (_lookahead_token.HasValue == false)
            {
                _lookahead_token = FetchToken(_provider);
            }

            //
            return _lookahead_token.GetValueOrDefault();
        }

        static readonly HashSet<Tokens> s_semi_reserved = new HashSet<Tokens>()
        {
            Tokens.T_INCLUDE,
            Tokens.T_INCLUDE_ONCE,
            Tokens.T_EVAL,
            Tokens.T_REQUIRE,
            Tokens.T_REQUIRE_ONCE,
            Tokens.T_LOGICAL_OR,
            Tokens.T_LOGICAL_XOR,
            Tokens.T_LOGICAL_AND,
            Tokens.T_INSTANCEOF,
            Tokens.T_NEW,
            Tokens.T_CLONE,
            Tokens.T_EXIT,
            Tokens.T_IF,
            Tokens.T_ELSEIF,
            Tokens.T_ELSE,
            Tokens.T_ENDIF,
            Tokens.T_ECHO,
            Tokens.T_DO,
            Tokens.T_WHILE,
            Tokens.T_ENDWHILE,
            Tokens.T_FOR,
            Tokens.T_ENDFOR,
            Tokens.T_FOREACH,
            Tokens.T_ENDFOREACH,
            Tokens.T_DECLARE,
            Tokens.T_ENDDECLARE,
            Tokens.T_AS,
            Tokens.T_TRY,
            Tokens.T_CATCH,
            Tokens.T_FINALLY,
            Tokens.T_THROW,
            Tokens.T_USE,
            Tokens.T_INSTEADOF,
            Tokens.T_GLOBAL,
            Tokens.T_VAR,
            Tokens.T_UNSET,
            Tokens.T_ISSET,
            Tokens.T_EMPTY,
            Tokens.T_CONTINUE,
            Tokens.T_GOTO,
            Tokens.T_FUNCTION,
            Tokens.T_STATIC,
            Tokens.T_ABSTRACT,
            Tokens.T_FINAL,
            Tokens.T_PRIVATE,
            Tokens.T_PROTECTED,
            Tokens.T_PUBLIC,
            Tokens.T_DEFAULT,
            Tokens.T_CONST,
            Tokens.T_CLASS,
            Tokens.T_INTERFACE,
            Tokens.T_TRAIT,
            Tokens.T_LIST,
            Tokens.T_SWITCH,
            Tokens.T_ENDSWITCH,
            Tokens.T_PRINT,
            Tokens.T_ARRAY,
            Tokens.T_CALLABLE,
            Tokens.T_IMPLEMENTS,
            Tokens.T_EXTENDS,
            Tokens.T_NAMESPACE,
            Tokens.T_CASE,
            Tokens.T_BREAK,
            Tokens.T_RETURN,
            Tokens.T_YIELD,
            Tokens.T_FN,
            Tokens.T_MATCH,
            Tokens.T_READONLY,
            Tokens.T_ENUM,
            Tokens.T_LINE,
            Tokens.T_FILE,
            Tokens.T_DIR,
            Tokens.T_NS_C,
            Tokens.T_CLASS_C,
            Tokens.T_TRAIT_C,
            Tokens.T_FUNC_C,
            Tokens.T_METHOD_C,
            Tokens.T_PROPERTY_C,
        };

        /// <summary>
        /// Effectively consumes next token from the <see cref="_provider"/>.
        /// </summary>
        TokenSnapshot FetchToken(TProvider lexer)
        {
            // advance lexer
            var t = (Tokens)lexer.GetNextToken();

            // create snapshot
            return new TokenSnapshot(t, ResolveTokenData(/*expected lexer in current state*/), lexer);
        }

        /// <summary>
        /// Get next token and store its actual position in the source unit.
        /// This implementation supports the functionality of zendlex, which skips empty nodes (open tag, comment, etc.).
        /// </summary>
        /// <returns>Next token.</returns>
        int ITokenProvider<SemanticValueType, Span>.GetNextToken()
        {
            for (; MoveNext();)
            {
                var token = CurrentToken;

                // original zendlex() functionality - skip open and close tags because they are not in the PHP grammar
                switch (CurrentToken)
                {
                    case Tokens.T_FN:
                        if (!HasFeatureSet(LanguageFeatures.Php74Set))
                        {
                            token = Tokens.T_STRING;
                        }
                        break;

                    case Tokens.T_MATCH:
                        if (!HasFeatureSet(LanguageFeatures.Php80Set))
                        {
                            token = Tokens.T_STRING;
                        }
                        break;

                    // reinterpret T_STRING if PHP < 8.1
                    case Tokens.T_READONLY:
                    case Tokens.T_ENUM:
                        if (!HasFeatureSet(LanguageFeatures.Php81Set))
                        {
                            token = Tokens.T_STRING;
                        }
                        break;

                    case Tokens.T_PROPERTY_C: // PHP >= 8.4
                        if (!HasFeatureSet(LanguageFeatures.Php84Set))
                        {
                            token = Tokens.T_STRING;
                        }
                        break;

                    case Tokens.T_PRIVATE_SET:
                    case Tokens.T_PUBLIC_SET:
                    case Tokens.T_PROTECTED_SET:
                        if (!HasFeatureSet(LanguageFeatures.Php84Set))
                        {
                            // error ?
                        }
                        break;
                }

                // semi_reserved:
                // (reinterpret T_NAME_QUALIFIED)
                if (HasFeatureSet(LanguageFeatures.Php80Set) && s_semi_reserved.Contains(token))
                {
                    // After "\", it is treated as identifier.
                    // See T_NAME_QUALIFIED in Zend.
                    //
                    // We don't tokenize the source code in the same way,
                    // since it would break backward compatibility with older parsers.

                    if (PreviousToken == Tokens.T_NS_SEPARATOR || GetLookaheadToken().Token == Tokens.T_NS_SEPARATOR)
                    {
                        token = Tokens.T_STRING;
                    }
                }

                //
                return (int)token;
            }

            //
            return (int)Tokens.EOF;
        }

        public void ReportError(string[] expectedTokens) => _provider.ReportError(expectedTokens);
    }
}
