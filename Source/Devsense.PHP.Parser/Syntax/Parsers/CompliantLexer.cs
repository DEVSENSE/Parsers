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
        #region ITokenProviderInternal

        DocCommentContainer ITokenProviderInternal<SemanticValueType, Span>.DocCommentList => _phpDocs;

        #endregion

        #region ITokenProvider

        public IDocBlock DocComment => _phpDocs.LastDocBlock;

        public Span TokenPosition => _provider.TokenPosition;

        public string TokenText => _provider.TokenText;

        public ReadOnlySpan<char> TokenTextSpan => _provider.TokenTextSpan;

        public ReadOnlyMemory<char> TokenSource => _provider.TokenSource;

        public SemanticValueType TokenValue => _provider.TokenValue;

        #endregion

        readonly ITokenProvider<SemanticValueType, Span> _provider;

        readonly DocCommentContainer _phpDocs = new DocCommentContainer();

        /// <summary>
        /// Lexer constructor that initializes all the necessary members
        /// </summary>
        /// <param name="provider">Underlying tokens provider.</param>
        public CompliantLexer(ITokenProvider<SemanticValueType, Span> provider)
        {
            _provider = provider;
        }

        /// <summary>Current documentary block with whitespace suffixed right after <c>*/</c>.</summary>
        IDocBlock _backup_doc_comment = null;

        /// <summary>nesting level of #[ ... ]</summary>
        int _backup_attribute_level = 0;

        /// <summary>
        /// Get next token and store its actual position in the source unit.
        /// This implementation supports the functionality of zendlex, which skips empty nodes (open tag, comment, etc.).
        /// </summary>
        /// <returns>Next token.</returns>
        int ITokenProvider<SemanticValueType, Span>.GetNextToken()
        {
            Tokens previous_token, token = 0;
            Span previous_token_span = Span.Invalid;

            for (; ;)
            {
                previous_token = token;
                previous_token_span = _provider.TokenPosition;

                token = (Tokens)_provider.GetNextToken();

                // original zendlex() functionality - skip open and close tags because they are not in the PHP grammar
                switch (token)
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
                            previous_token,
                            previous_token_span
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
