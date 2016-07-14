using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PHP.Core.Text;
using PHP.Core.AST;
using PHP.Syntax;
using System.IO;
using System.Diagnostics;

namespace PhpParser.Parser
{
    public sealed class EmptyErrorSink : ErrorSink
    {

        public EmptyErrorSink()
            : base()
        {
        }

        protected override bool Add(int id, string message, ErrorSeverity severity, int group, string fullPath, ErrorPosition pos)
        {
            return true;
        }
    }

    public partial class Parser : ICommentsSink, IScannerHandler
    {
        #region Reductions Sinks

        private sealed class NullReductionsSink : IReductionsSink
        {
            void IReductionsSink.InclusionReduced(Parser/*!*/ parser, IncludingEx/*!*/ incl)
            {
            }

            void IReductionsSink.FunctionDeclarationReduced(Parser/*!*/ parser, FunctionDecl/*!*/ decl)
            {
            }

            void IReductionsSink.TypeDeclarationReduced(Parser/*!*/ parser, TypeDecl/*!*/ decl)
            {
            }

            void IReductionsSink.GlobalConstantDeclarationReduced(Parser/*!*/ parser, GlobalConstantDecl/*!*/ decl)
            {
            }

            void IReductionsSink.NamespaceDeclReduced(Parser parser, NamespaceDecl decl)
            {
            }

            void IReductionsSink.LambdaFunctionReduced(Parser parser, LambdaFunctionExpr decl)
            {
            }
        }

        public sealed class ReductionsCounter : IReductionsSink
        {
            public int InclusionCount { get { return _inclusionCount; } }
            private int _inclusionCount = 0;

            public int FunctionCount { get { return _functionCount; } }
            private int _functionCount = 0;

            public int TypeCount { get { return _typeCount; } }
            private int _typeCount = 0;

            public int ConstantCount { get { return _constantCount; } }
            private int _constantCount = 0;

            void IReductionsSink.InclusionReduced(Parser/*!*/ parser, IncludingEx/*!*/ incl)
            {
                _inclusionCount++;
            }

            void IReductionsSink.FunctionDeclarationReduced(Parser/*!*/ parser, FunctionDecl/*!*/ decl)
            {
                _functionCount++;
            }

            void IReductionsSink.TypeDeclarationReduced(Parser/*!*/ parser, TypeDecl/*!*/ decl)
            {
                _typeCount++;
            }

            void IReductionsSink.GlobalConstantDeclarationReduced(Parser/*!*/ parser, GlobalConstantDecl/*!*/ decl)
            {
                _constantCount++;
            }

            void IReductionsSink.NamespaceDeclReduced(Parser parser, NamespaceDecl decl)
            {
            }

            void IReductionsSink.LambdaFunctionReduced(Parser parser, LambdaFunctionExpr decl)
            {
            }
        }

        #endregion

        SourceUnit _sourceUnit;
        TextReader _reader;
        ErrorSink _errors;
        LanguageFeatures _features;
        IReductionsSink _reductionsSink;
        Lexer _lexer;
        Scope _currentScope;

        /// <summary>
        /// The root of AST.
        /// </summary>
        private GlobalCode _astRoot;

        protected sealed override int EofToken
        {
            get { return (int)Tokens.END; }
        }

        protected sealed override int ErrorToken
        {
            get { return (int)Tokens.T_ERROR; }
        }

        protected override Span CombinePositions(Span first, Span last)
        {
            if (last.IsValid)
            {
                if (first.IsValid)
                    return Span.Combine(first, last);
                else
                    return last;
            }
            else
                return first;
        }

        public GlobalCode Parse(SourceUnit/*!*/ sourceUnit,
            TextReader/*!*/ reader, ErrorSink/*!*/ errors,
            IReductionsSink reductionsSink, Lexer.LexicalStates initialLexicalState,
            LanguageFeatures features, int positionShift = 0)
        {
            Debug.Assert(reader != null && errors != null);

            // initialization:
            this._sourceUnit = sourceUnit;
            this._errors = errors;
            this._features = features;
            this._reader = reader;
            this._reductionsSink = reductionsSink ?? new NullReductionsSink();
            //InitializeFields();

            this._lexer = new Lexer(reader, sourceUnit, errors, this, this, features, positionShift) { CurrentLexicalState = initialLexicalState };
            this._lexer.CurrentLexicalState = initialLexicalState;
            this._currentScope = new Scope(1); // starts assigning scopes from 2 (1 is reserved for prepended inclusion)

            base.Scanner = this._lexer;
            base.Parse();

            GlobalCode result = _astRoot;

            // clean and let GC collect unused AST and other stuff:
            //ClearFields();

            return result;
        }

#region ICommentsSink and IScannerHandler code

        void ICommentsSink.OnLineComment(Lexer scanner, TextSpan span)
        {
            //_commentSink.OnLineComment(scanner, span);
        }

        void ICommentsSink.OnComment(Lexer scanner, TextSpan span)
        {
            //_commentSink.OnComment(scanner, span);
        }

        void ICommentsSink.OnPhpDocComment(Lexer scanner, PHPDocBlock phpDocBlock)
        {
            // handle the next non-whitespace token so we'll know span of the DOC comment including the following whitespace
            //_scannerHandler = new HandleDocComment(this, phpDocBlock, _scannerHandler);

            //
            //_commentSink.OnPhpDocComment(scanner, phpDocBlock);
        }

        void ICommentsSink.OnOpenTag(Lexer scanner, TextSpan span)
        {
            //_commentSink.OnOpenTag(scanner, span);
        }

        void ICommentsSink.OnCloseTag(Lexer scanner, TextSpan span)
        {
            //_commentSink.OnCloseTag(scanner, span);
        }

        void IScannerHandler.OnNextToken(Tokens token, char[] buffer, int tokenStart, int tokenLength)
        {
            //_scannerHandler.OnNextToken(token, buffer, tokenStart, tokenLength);
        }

#endregion
    }
}
