// Copyright(c) DEVSENSE s.r.o.
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// PHP lexer generated according to the PhpLexer.l grammar.
    /// Special implementation of the PHP lexer that skips empty nodes (open tag, comment, etc.), which is required by the PHP grammar.
    /// </summary>
    public class CompliantLexer : ITokenProviderInternal<SemanticValueType, Span> //, IEnumerator<TokenSnapshot>
    {
        #region TokenSnapshot

        /// <summary>
        /// Current token information. used for lookahead and lookback.
        /// </summary>
        readonly struct TokenSnapshot<ValueType, PositionType>
        {
            public readonly Tokens Token;
            public readonly PositionType TokenPosition;
            public readonly ReadOnlyMemory<char> TokenSource;
            public readonly ValueType TokenValue;

            public TokenSnapshot(Tokens token, ITokenProvider<ValueType, PositionType> lexer)
            {
                this.Token = token;
                this.TokenPosition = lexer.TokenPosition;
                this.TokenSource = lexer.TokenSource;
                this.TokenValue = lexer.TokenValue;
            }
        }

        #endregion

        #region ITokenProviderInternal

        DocCommentContainer ITokenProviderInternal<SemanticValueType, Span>.DocCommentList => _phpDocs;

        #endregion

        #region ITokenProvider

        public IDocBlock DocComment => _phpDocs.LastDocBlock;

        public Span TokenPosition => _current_token.TokenPosition;

        public string TokenText => TokenSource.ToString();

        public ReadOnlySpan<char> TokenTextSpan => TokenSource.Span;

        public ReadOnlyMemory<char> TokenSource => _current_token.TokenSource;

        public SemanticValueType TokenValue => _current_token.TokenValue;

        #endregion

        readonly ITokenProvider<SemanticValueType, Span> _provider;

        readonly DocCommentContainer _phpDocs = new DocCommentContainer();

        readonly LanguageFeatures _features;

        /// <summary>
        /// Lexer constructor that initializes all the necessary members
        /// </summary>
        /// <param name="provider">Underlying tokens provider.</param>
        /// <param name="language">Language features.</param>
        public CompliantLexer(ITokenProvider<SemanticValueType, Span> provider, LanguageFeatures language = LanguageFeatures.Basic)
        {
            _provider = provider;
            _features = language;
        }

        /// <summary>Current documentary block with whitespace suffixed right after <c>*/</c>.</summary>
        IDocBlock _backup_doc_comment = null;

        /// <summary>nesting level of #[ ... ]</summary>
        int _backup_attribute_level = 0;

        TokenSnapshot<SemanticValueType, Span>? _previous_token, _lookahead_token;
        TokenSnapshot<SemanticValueType, Span> _current_token;

        bool HasFeatureSet(LanguageFeatures fset) => (_features & fset) == fset;

        /// <summary>
        /// Advances <see cref="_current_token"/> to the next token.
        /// </summary>
        /// <returns>Value indicating there was another token (i.e. it was not <c>EOF</c>).</returns>
        bool MoveNext()
        {
            _previous_token = _current_token;

            if (_lookahead_token.HasValue) // already fetched
            {
                _current_token = _lookahead_token.GetValueOrDefault();
                _lookahead_token = default(TokenSnapshot<SemanticValueType, Span>?);
            }
            else
            {
                _current_token = FetchToken(_provider);
            }

            //
            return _current_token.Token != Tokens.END;
        }

        TokenSnapshot<SemanticValueType, Span> Current => _current_token;

        Tokens CurrentToken => _current_token.Token;

        Tokens PreviousToken => _previous_token.GetValueOrDefault().Token;

        /// <summary>
        /// Gets token AFTER the the current one.
        /// Current token must be fetched, and underlying lexer must be in valid state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Underlying lexer is not in valid state.</exception>
        TokenSnapshot<SemanticValueType, Span> GetLookaheadToken()
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
        static TokenSnapshot<ValueType, PositionType> FetchToken<ValueType, PositionType>(ITokenProvider<ValueType, PositionType> lexer)
        {
            // advance lexer
            var t = (Tokens)lexer.GetNextToken();
            return new TokenSnapshot<ValueType, PositionType>(t, lexer);
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
                    case Tokens.T_OPEN_TAG:
                        continue;
                    case Tokens.T_CLOSE_TAG:
                        token = Tokens.T_SEMI; /* implicit ; */
                        break;
                    case Tokens.T_OPEN_TAG_WITH_ECHO:
                        token = Tokens.T_ECHO;
                        break;

                    case Tokens.T_DOC_COMMENT:
                        _backup_doc_comment = _phpDocs.Append(
                            _provider.DocComment,
                            _previous_token.GetValueOrDefault().Token,
                            _previous_token.GetValueOrDefault().TokenPosition
                        );
                        // TODO: nullify once consumed
                        continue;
                    case Tokens.T_WHITESPACE:
                    case Tokens.T_COMMENT:
                        UpdateDocCommentExtent(TokenPosition);
                        continue;

                    //case Tokens.T_ATTRIBUTE:
                    //    if (!HasFeatureSet(LanguageFeatures.Php80Set))
                    //    {
                    //        token = Tokens.T_COMMENT; // should be merged with the rest of the line
                    //        goto token;
                    //    }
                    //    break;

                    case Tokens.T_ATTRIBUTE:
                        _backup_attribute_level++;
                        break;

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

                // extend the span of preceding doc comment block over the #[] attributes,
                // so it can be properly applied to the next language element
                if (_backup_attribute_level > 0)
                {
                    UpdateDocCommentExtent(TokenPosition);

                    // update the nesting level eventually
                    if (token == Tokens.T_LBRACKET)
                        _backup_attribute_level++;
                    else if (token == Tokens.T_RBRACKET)
                        _backup_attribute_level--;
                }

                //
                return (int)token;
            }

            //
            return (int)Tokens.EOF;
        }

        void UpdateDocCommentExtent(Span whiteSpan)
        {
            var e = _backup_doc_comment;
            if (e != null && e.Extent.End == whiteSpan.Start)
            {
                e.Extent = Span.FromBounds(e.Extent.Start, whiteSpan.End);
            }
        }

        public void ReportError(string[] expectedTokens) => _provider.ReportError(expectedTokens);
    }
}
