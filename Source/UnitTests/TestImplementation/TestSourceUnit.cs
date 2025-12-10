using Devsense.PHP.Errors;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTests.TestImplementation
{
    internal class TestSourceUnit : CodeSourceUnit
    {
        private CollectionLexer _lexer;
        private LanguageFeatures _features;
        public List<Comment> Comments => _lexer.Comments;

        public CollectionLexer SourceLexer => _lexer;

        public TestSourceUnit(string/*!*/ code, string/*!*/ filePath,
            Encoding/*!*/ encoding,
            Lexer.LexicalStates initialState = Lexer.LexicalStates.INITIAL,
            LanguageFeatures features = LanguageFeatures.Basic)
            : base(code, filePath, encoding, initialState, features)
        {
            _features = features;
        }

        public override void Parse(INodesFactory<LangElement, Span> factory, IErrorSink<Span> errors, IErrorRecovery recovery = null)
        {
            _lexer = new CollectionLexer(this.Code.AsMemory(), errors);
            this.Ast = (GlobalCode)new Parser().Parse(_lexer, factory, _features, errors, recovery);
        }
    }
}
