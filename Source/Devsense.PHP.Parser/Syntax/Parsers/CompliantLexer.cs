﻿// Copyright(c) DEVSENSE s.r.o.
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
        /// <param name="provider">Underlying tokens provider.</param>
        /// <param name="language">Language features.</param>
        public CompliantLexer(ITokenProvider<SemanticValueType, Span> provider, LanguageFeatures language = LanguageFeatures.Basic)
        {
            _provider = provider;
            _features = language;
        }

        IDocBlock _backup_doc_comment = null;
        int _backup_attribute_level = 0; // nesting level of #[ ... ]

        Tokens _backup_token = Tokens.EOF;

        public IDocBlock DocComment => _phpDocs.LastDocBlock;

        DocCommentContainer IParserTokenProvider<SemanticValueType, Span>.DocCommentList => _phpDocs;

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
            var token = _backup_token;
            
            for (; ; )
            {
                var prev_span = TokenPosition;
                var prev_token = token;
                
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
                        _backup_doc_comment = _phpDocs.Append(_provider.DocComment, prev_token, prev_span);
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

                    case Tokens.T_PROPERTY_C: // PHP >= 8.4
                        if (!HasFeatureSet(LanguageFeatures.Php84Set) ||
                            _backup_token == Tokens.T_NS_SEPARATOR)
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

                    // reinterpret T_NAME_QUALIFIED
                    case Tokens.T_GLOBAL:
                    case Tokens.T_STATIC:
                    case Tokens.T_ABSTRACT:
                    case Tokens.T_FINAL:
                    case Tokens.T_PRIVATE:
                    case Tokens.T_PROTECTED:
                    case Tokens.T_PUBLIC:
                    case Tokens.T_DEFAULT:
                    case Tokens.T_EVAL:
                    case Tokens.T_CONST:
                    case Tokens.T_CLASS:
                    case Tokens.T_INTERFACE:
                    case Tokens.T_TRAIT:
                    case Tokens.T_LIST:
                    case Tokens.T_SWITCH:
                    case Tokens.T_PRINT:
                    case Tokens.T_CLONE:
                    case Tokens.T_ARRAY:
                    case Tokens.T_DO:
                    case Tokens.T_WHILE:
                    case Tokens.T_FOR:
                    case Tokens.T_IF:
                    case Tokens.T_NAMESPACE:
                    case Tokens.T_FUNCTION:
                    case Tokens.T_BREAK:
                    case Tokens.T_CONTINUE:
                    case Tokens.T_RETURN:
                    case Tokens.T_THROW:
                    case Tokens.T_TRY:
                    case Tokens.T_USE:
                    case Tokens.T_VAR:
                    case Tokens.T_YIELD:
                        if (_backup_token == Tokens.T_NS_SEPARATOR || // after "\", it is treated as identifier. See T_NAME_QUALIFIED in Zend. We don't, since it would break backward compatibility with older parsers.
                            _backup_token == Tokens.T_NAMESPACE ||
                            _backup_token == Tokens.T_IMPLEMENTS || // implements trait/X
                            _backup_token == Tokens.T_EXTENDS)    // extends trait/X
                        {
                            token = Tokens.T_STRING;
                        }
                        break;
                }

                if (_backup_attribute_level > 0)
                {
                    // extend the span of preceding doc comment block
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

        void UpdateDocCommentExtent(Span whiteSpan)
        {
            var e = _backup_doc_comment;
            if (e != null && e.Extent.End == whiteSpan.Start)
            {
                e.Extent = Span.FromBounds(e.Extent.Start, whiteSpan.End);
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
