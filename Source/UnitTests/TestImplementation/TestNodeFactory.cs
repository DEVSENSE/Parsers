using Devsense.PHP.Errors;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.TestImplementation
{
    internal class TestNodeFactory : BasicNodesFactory
    {

        public IReadOnlyList<MethodDecl> Methods => _methods;
        readonly List<MethodDecl> _methods = new List<MethodDecl>();

        public IReadOnlyList<FunctionDecl> Functions => _functions;
        readonly List<FunctionDecl> _functions = new List<FunctionDecl>();

        protected override IErrorSink<Span> ErrorSink => _errors;
        readonly IErrorSink<Span> _errors;

        public TestNodeFactory(SourceUnit sourceUnit, IErrorSink<Span> errors) : base(sourceUnit)
        {
            _errors = errors;
        }

        public override LangElement Method(Span span, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Span returnTypeSpan, string name, Span nameSpan, FormalTypeParam[] typeParamsOpt, FormalParam[] formalParams, Span formalParamsSpan, ActualParam[] baseCtorParams, LangElement body)
        {
            var m = base.Method(span, aliasReturn, attributes, returnType, returnTypeSpan, name, nameSpan, typeParamsOpt, formalParams, formalParamsSpan, baseCtorParams, body);

            Assert.IsNotNull(m);

            _methods.Add((MethodDecl)m);

            return m;
        }

        public override LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Name name, Span nameSpan, FormalTypeParam[] typeParamsOpt, FormalParam[] formalParams, Span formalParamsSpan, LangElement body)
        {
            var f = base.Function(span, conditional, aliasReturn, attributes, returnType, name, nameSpan, typeParamsOpt, formalParams, formalParamsSpan, body);

            Assert.IsNotNull(f);

            _functions.Add((FunctionDecl)f);

            return f;
        }
    }
}
