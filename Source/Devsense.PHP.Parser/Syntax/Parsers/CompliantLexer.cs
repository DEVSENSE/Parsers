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

        readonly DocCommentList _phpDocs = new DocCommentList();

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

        PHPDocBlock backupDocBlock = null;

        public PHPDocBlock DocBlock { get { return _phpDocs.LastDocBlock; } set { } }

        public DocCommentList DocBlockList { get { return _phpDocs; } }

        public Span TokenPosition => _provider.TokenPosition;

        public string TokenText => _provider.TokenText;

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
            int docBlockExtend = -1;
            for (; ; )
            {
                Tokens token = (Tokens)_provider.GetNextToken();

                // origianl zendlex() functionality - skip open and close tags because they are not in the PHP grammar
                switch (token)
                {
                    case Tokens.T_DOC_COMMENT:
                        SaveDocComment(docBlockExtend);
                        backupDocBlock = _provider.DocBlock;
                        docBlockExtend = TokenPosition.End;
                        continue;
                    case Tokens.T_WHITESPACE:
                    case Tokens.T_COMMENT:
                        docBlockExtend = TokenPosition.End;
                        continue;
                    case Tokens.T_OPEN_TAG:
                        continue;
                    case Tokens.T_CLOSE_TAG:
                        token = Tokens.T_SEMI; /* implicit ; */
                        break;
                    case Tokens.T_OPEN_TAG_WITH_ECHO:
                        token = Tokens.T_ECHO;
                        break;

                    case Tokens.T_FN:
                        if (!HasFeatureSet(LanguageFeatures.Php74Set))
                        {
                            TokenValue = new SemanticValueType() { String = TokenText, };
                            token = Tokens.T_STRING;
                        }
                        break;

                        //case Tokens.T_MATCH:
                        //    if (!HasFeatureSet(LanguageFeatures.Php80Set))
                        //    {
                        //        token = Tokens.T_STRING;
                        //    }
                        //    break;
                }
                SaveDocComment(docBlockExtend);

                return (int)token;
            }
        }

        void SaveDocComment(int extend)
        {
            if (backupDocBlock != null)
            {
                _phpDocs.AppendBlock(backupDocBlock, extend);
                backupDocBlock = null;
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
