using Devsense.PHP.Errors;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Devsense.PHP.Syntax.Ast.EncapsedExpression;

namespace UnitTests.TestImplementation
{
    internal class TestNodeFactory : BasicNodesFactory
    {

        public IReadOnlyList<MethodDecl> Methods => _methods;
        readonly List<MethodDecl> _methods = new List<MethodDecl>();

        public IReadOnlyList<FunctionDecl> Functions => _functions;
        readonly List<FunctionDecl> _functions = new List<FunctionDecl>();

        public IReadOnlyList<NamedTypeDecl> Types => _types;
        readonly List<NamedTypeDecl> _types = new List<NamedTypeDecl>();

        public IReadOnlyList<StringEncapsedExpression> HereDocs => _heredocs;
        readonly List<StringEncapsedExpression> _heredocs = new List<StringEncapsedExpression>();

        protected override IErrorSink<Span> ErrorSink => _errors;
        readonly IErrorSink<Span> _errors;

        public TestNodeFactory(SourceUnit sourceUnit, IErrorSink<Span> errors) : base(sourceUnit)
        {
            _errors = errors;
        }

        public override LangElement Type(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, INamedTypeRef baseClassOpt, INamedTypeRef[] implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            var t = base.Type(span, headingSpan, conditional, attributes, name, nameSpan, typeParamsOpt, baseClassOpt, implements, members, bodySpan);

            Assert.NotNull(t);

            _types.Add((NamedTypeDecl)t);

            return t;
        }

        public override LangElement Method(Span span, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Span returnTypeSpan, string name, Span nameSpan, FormalTypeParam[] typeParamsOpt, FormalParam[] formalParams, Span formalParamsSpan, ActualParam[] baseCtorParams, LangElement body)
        {
            var m = base.Method(span, aliasReturn, attributes, returnType, returnTypeSpan, name, nameSpan, typeParamsOpt, formalParams, formalParamsSpan, baseCtorParams, body);

            Assert.NotNull(m);

            _methods.Add((MethodDecl)m);

            return m;
        }

        public override LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Name name, Span nameSpan, FormalTypeParam[] typeParamsOpt, FormalParam[] formalParams, Span formalParamsSpan, LangElement body)
        {
            var f = base.Function(span, conditional, aliasReturn, attributes, returnType, name, nameSpan, typeParamsOpt, formalParams, formalParamsSpan, body);

            Assert.NotNull(f);

            _functions.Add((FunctionDecl)f);

            return f;
        }

        public override LangElement HeredocExpression(Span span, LangElement expression, Tokens quoteStyle, Lexer.HereDocTokenValue heredoc)
        {
            var e = base.HeredocExpression(span, expression, quoteStyle, heredoc);

            _heredocs.Add((StringEncapsedExpression)e);

            return e;
        }
    }
}
