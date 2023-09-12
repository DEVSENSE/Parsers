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

using System;
using System.Collections.Generic;
using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// PHP lexer generated according to the PhpLexer.l grammar.
    /// Special implementation of the PHP lexer that skips empty nodes (open tag, comment, etc.), which is required by the PHP grammar.
    /// </summary>
    public class CompliantLexer : IParserTokenProvider<SemanticValueType, Span>
    {
        readonly ITokenProvider<SemanticValueType, Span> _provider;

        readonly DocCommentContainer _phpDocs = new DocCommentContainer();

        readonly LanguageFeatures _features;

        /// <summary>
        /// Lexer constructor that initializes all the necessary members
        /// </summary>
        /// <param name="provider">Underlaying tokens provider.</param>
        /// <param name="language">Language features.</param>
        public CompliantLexer(ITokenProvider<SemanticValueType, Span> provider, LanguageFeatures language = LanguageFeatures.Basic)
        {
            _provider = provider;
            _features = language;
        }

        IPhpDocExtent _backup_doc_comment = null;
        int _backup_attribute_level = 0; // nesting level of #[ ... ]

        Tokens _backup_token = Tokens.EOF;

        public IDocBlock DocComment
        {
            get { return _phpDocs.LastDocBlock; }
            set { /*not supported*/ }
        }

        DocCommentContainer IParserTokenProvider<SemanticValueType, Span>.DocCommentList { get { return _phpDocs; } }

        public Span TokenPosition => _provider.TokenPosition;

        public string TokenText => _provider.TokenText;

        public ReadOnlySpan<char> TokenTextSpan => _provider.TokenTextSpan;

        public SemanticValueType TokenValue
        {
            get => _provider.TokenValue;
            set => _provider.TokenValue = value;
        }

        bool HasFeatureSet(LanguageFeatures fset) => (_features & fset) == fset;

        /// <summary>
        /// Get next token and store its actual position in the source unit.
        /// This implementation supports the functionality of zendlex, which skips empty nodes (open tag, comment, etc.).
        /// </summary>
        /// <returns>Next token.</returns>
        public int GetNextToken()
        {
            for (; ; )
            {
                Tokens token = (Tokens)_provider.GetNextToken();

                // original zendlex() functionality - skip open and close tags because they are not in the PHP grammar
                switch (token)
                {
                    case Tokens.T_DOC_COMMENT:
                        _backup_doc_comment = _phpDocs.Append(_provider.DocComment);
                        // TODO: nullify once consumed
                        continue;
                    case Tokens.T_WHITESPACE:
                    case Tokens.T_COMMENT:
                        UpdateDocCommentExtent(TokenPosition);
                        continue;
                    case Tokens.T_OPEN_TAG:
                        continue;
                    case Tokens.T_CLOSE_TAG:
                        token = Tokens.T_SEMI; /* implicit ; */
                        break;
                    case Tokens.T_OPEN_TAG_WITH_ECHO:
                        token = Tokens.T_ECHO;
                        break;

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
                        if (!HasFeatureSet(LanguageFeatures.Php74Set) ||
                            _backup_token == Tokens.T_NS_SEPARATOR)
                        {
                            token = Tokens.T_STRING;
                        }
                        break;

                    case Tokens.T_MATCH:
                        if (!HasFeatureSet(LanguageFeatures.Php80Set) ||
                            _backup_token == Tokens.T_NS_SEPARATOR)
                        {
                            token = Tokens.T_STRING;
                        }
                        break;

                    // reinterpret T_NAME_QUALIFIED
                    // reinterpret T_STRING if PHP < 8.1
                    case Tokens.T_READONLY:
                    case Tokens.T_ENUM:
                        if (!HasFeatureSet(LanguageFeatures.Php81Set) ||
                            _backup_token == Tokens.T_NS_SEPARATOR) // after "\", it is treated as identifier. See T_NAME_QUALIFIED in Zend. We don't, since it would break backward compatibility with older parsers.
                        {
                            token = Tokens.T_STRING;
                        }
                        break;

                    // reinterpret T_NAME_QUALIFIED
                    case Tokens.T_GLOBAL:
                    case Tokens.T_STATIC:
                    case Tokens.T_DEFAULT:
                    case Tokens.T_EVAL:
                    case Tokens.T_CONST:
                    case Tokens.T_INTERFACE:
                    case Tokens.T_TRAIT:
                    case Tokens.T_LIST:
                    case Tokens.T_SWITCH:
                        if (_backup_token == Tokens.T_NS_SEPARATOR) // after "\", it is treated as identifier. See T_NAME_QUALIFIED in Zend. We don't, since it would break backward compatibility with older parsers.
                        {
                            token = Tokens.T_STRING;
                        }
                        break;
                }

                if (_backup_attribute_level > 0)
                {
                    // extend the span of preceeding doc comment block
                    // so it can be properly applied to the next language element
                    UpdateDocCommentExtent(TokenPosition);

                    // update the nesting level eventually
                    if (token == Tokens.T_LBRACKET)
                        _backup_attribute_level++;
                    else if (token == Tokens.T_RBRACKET)
                        _backup_attribute_level--;
                }

                //
                _backup_token = token;

                //
                return (int)token;
            }
        }

        void UpdateDocCommentExtent(Span whitespan)
        {
            var doccomment = _backup_doc_comment;
            if (doccomment != null && doccomment.Extent.End == whitespan.Start)
            {
                doccomment.Extent = Span.FromBounds(doccomment.Extent.Start, whitespan.End);
            }
        }

        public void ReportError(string[] expectedTokens)
        {
            _provider.ReportError(expectedTokens);
        }

        public CompleteToken PreviousToken => CompleteToken.Empty;

        public void AddNextTokens(IList<CompleteToken> tokensBuffer, CompleteToken previousToken) { }
    }
}
