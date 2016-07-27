using PHP.Core.AST;
using PHP.Core.Text;
using PHP.Syntax;
using PhpParser;
using PhpParser.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhpParser
{
    /// <summary>
    /// Nodes factory used by <see cref="Parser.Parser"/>.
    /// </summary>
    public class BasicNodesFactory : INodesFactory<LangElement, Span>
    {
        SourceUnit _sourceUnit;
        List<Tuple<Span, ErrorInfo, string[]>> _errors = new List<Tuple<Span, ErrorInfo, string[]>>();
        public List<Tuple<Span, ErrorInfo, string[]>> Errors { get { return _errors; } }

        public void Error(Span span, ErrorInfo info, params string[] argsOpt)
        {
            Errors.Add(new Tuple<Span, ErrorInfo, string[]>(span, info, argsOpt));
        }

        List<T> ConvertList<T>(IEnumerable<LangElement> list) where T : LangElement
        {
            Debug.Assert(list.All(s => s == null || s is T), "List of LangELements contains node that is not valid!");
            return list.Select(s => (T)s).ToList();
        }

        public BasicNodesFactory(SourceUnit sourceUnit)
        {
            _sourceUnit = sourceUnit;
        }

        public LangElement ArrayItem(Span span, LangElement expression, LangElement indexOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement Assert(Span span, LangElement assertion, LangElement failureOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement Assignment(Span span, LangElement target, LangElement value, Operations assignOp)
        {
            if (assignOp == Operations.AssignRef)
                return new ValueAssignEx(span, Operations.AssignRef, (VariableUse)target, (Expression)value);
            else
                return new ValueAssignEx(span, assignOp, (VariableUse)target, (Expression)value);
        }

        public LangElement BinaryOperation(Span span, Operations operation, LangElement leftExpression, LangElement rightExpression)
        {
            return new BinaryEx(span, operation, (Expression)leftExpression, (Expression)rightExpression);
        }

        public LangElement Block(Span span, IEnumerable<LangElement> statements)
        {
            throw new NotImplementedException();
        }

        public LangElement BlockComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public LangElement Call(Span span, LangElement nameExpr, CallSignature signature, TypeRef typeRef)
        {
            throw new NotImplementedException();
        }

        public LangElement Call(Span span, LangElement nameExpr, CallSignature signature, LangElement memberOfOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement Call(Span span, Name name, Span nameSpan, CallSignature signature, TypeRef typeRef)
        {
            throw new NotImplementedException();
        }

        public LangElement Call(Span span, QualifiedName name, QualifiedName? nameFallback, Span nameSpan, CallSignature signature, LangElement memberOfOpt)
        {
            if (memberOfOpt == null)
                return new DirectFcnCall(span, name, nameFallback, nameSpan, signature.Parameters.ToList(), signature.GenericParams.ToList());
            else
                return new IndirectFcnCall(span, (Expression)memberOfOpt, signature.Parameters.ToList(), signature.GenericParams.ToList());
        }

        public LangElement ClassConstDecl(Span span, VariableName name, LangElement initializer)
        {
            throw new NotImplementedException();
        }

        public LangElement ColonBlock(Span span, IEnumerable<LangElement> statements, Tokens endToken)
        {
            throw new NotImplementedException();
        }

        public LangElement Concat(Span span, IEnumerable<LangElement> expressions)
        {
            throw new NotImplementedException();
        }

        public LangElement DeclList(Span span, PhpMemberAttributes attributes, IEnumerable<LangElement> decls)
        {
            throw new NotImplementedException();
        }

        public LangElement Do(Span span, LangElement body, LangElement cond)
        {
            throw new NotImplementedException();
        }

        public LangElement Echo(Span span, IEnumerable<LangElement> parameters)
        {
            return new EchoStmt(span, ConvertList<Expression>(parameters));
        }

        public LangElement Eval(Span span, LangElement code)
        {
            throw new NotImplementedException();
        }

        public LangElement Exit(Span span, LangElement statusOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement FieldDecl(Span span, VariableName name, LangElement initializerOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement For(Span span, IEnumerable<LangElement> init, IEnumerable<LangElement> cond, IEnumerable<LangElement> action, LangElement body)
        {
            throw new NotImplementedException();
        }

        public LangElement Foreach(Span span, LangElement enumeree, ForeachVar keyOpt, ForeachVar value, LangElement body)
        {
            throw new NotImplementedException();
        }

        public LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, QualifiedName? returnType, Span returnTypeSpan, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, LangElement body)
        {
            throw new NotImplementedException();
        }

        public LangElement GlobalCode(Span span, IEnumerable<LangElement> statements, NamingContext context)
        {
            _sourceUnit.Naming = context;
            return new GlobalCode(ConvertList<Statement>(statements), _sourceUnit);
        }

        public LangElement GlobalConstDecl(Span span, bool conditional, VariableName name, LangElement initializer)
        {
            return new GlobalConstantDecl(_sourceUnit, span, conditional, name.Value, (Expression)initializer);
        }

        public LangElement Goto(Span span, string label, Span labelSpan)
        {
            throw new NotImplementedException();
        }

        public LangElement HaltCompiler(Span span)
        {
            return new HaltCompiler(span);
        }

        public LangElement If(Span span, LangElement cond, LangElement body, LangElement elseOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement Inclusion(Span span, bool conditional, InclusionTypes type, LangElement fileNameExpression)
        {
            throw new NotImplementedException();
        }

        public LangElement IncrementDecrement(Span span, LangElement refexpression, bool inc, bool post)
        {
            throw new NotImplementedException();
        }

        public LangElement InlineHtml(Span span, string html)
        {
            return new EchoStmt(span, html);
        }

        public LangElement InstanceOf(Span span, LangElement expression, TypeRef typeRef)
        {
            throw new NotImplementedException();
        }

        public LangElement Jump(Span span, JumpStmt.Types type, LangElement exprOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement Label(Span span, string label, Span labelSpan)
        {
            throw new NotImplementedException();
        }

        public LangElement LineComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public LangElement List(Span span, IEnumerable<LangElement> targets)
        {
            if (targets.All(e => e is GlobalConstantDecl))
                return new GlobalConstDeclList(span, ConvertList<GlobalConstantDecl>(targets), null);
            if (targets.All(e => e is ClassConstantDecl))
                return new ConstDeclList(span, ConvertList<ClassConstantDecl>(targets), null);
            throw new NotImplementedException();
        }

        public LangElement Literal(Span span, object value)
        {
            if (value is long)
                return new LongIntLiteral(span, (long)value);
            else if (value is double)
                return new DoubleLiteral(span, (double)value);
            else if (value is string)
                return new StringLiteral(span, (string)value);
            else if (value is byte[])
                return new BinaryStringLiteral(span, (byte[])value);
            else if (value is bool)
                return new BoolLiteral(span, (bool)value);
            else if (value == null)
                return new NullLiteral(span);
            throw new NotImplementedException();
        }

        public LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, IEnumerable<LangElement> statements, NamingContext context)
        {
            NamespaceDecl space = new NamespaceDecl(span, name ?? new QualifiedName(Name.EmptyBaseName, Name.EmptyNames), statements == null);
            space.Naming = context;
            space.Statements = ConvertList<Statement>(statements);
            return space;
        }

        public LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, LangElement block, NamingContext context)
        {
            NamespaceDecl space = new NamespaceDecl(span, name ?? new QualifiedName(Name.EmptyBaseName, Name.EmptyNames), block == null);
            space.Naming = context;
            space.Statements = new List<Statement>() { (Statement)block };
            return space;
        }

        public LangElement New(Span span, TypeRef classNameRef, IEnumerable<ActualParam> argsOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement NewArray(Span span, IEnumerable<Item> itemsOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement ParenthesisExpression(Span span, LangElement expression)
        {
            throw new NotImplementedException();
        }

        public LangElement PHPDoc(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public LangElement Shell(Span span, LangElement command)
        {
            throw new NotImplementedException();
        }

        public LangElement Switch(Span span, LangElement value, LangElement block)
        {
            throw new NotImplementedException();
        }

        public LangElement TraitUse(Span span, IEnumerable<QualifiedName> traits, IEnumerable<TraitsUse.TraitAdaptation> adaptations)
        {
            throw new NotImplementedException();
        }

        public LangElement TryCatch(Span span, LangElement body, IEnumerable<CatchItem> catches, LangElement finallyBlockOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement Type(Span span, bool conditional, PhpMemberAttributes attributes, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, Tuple<GenericQualifiedName, Span> baseClassOpt, IEnumerable<Tuple<GenericQualifiedName, Span>> implements, IEnumerable<LangElement> members, Span blockSpan)
        {
            throw new NotImplementedException();
        }

        public LangElement UnaryOperation(Span span, Operations operation, LangElement expression)
        {
            return new UnaryEx(operation, (Expression)expression);
        }

        public LangElement Variable(Span span, LangElement nameExpr, TypeRef typeRef)
        {
            throw new NotImplementedException();
        }

        public LangElement Variable(Span span, LangElement nameExpr, LangElement memberOfOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement Variable(Span span, VariableName name, TypeRef typeRef)
        {
            throw new NotImplementedException();
        }

        public LangElement Variable(Span span, VariableName name, LangElement memberOfOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement While(Span span, LangElement cond, LangElement body)
        {
            throw new NotImplementedException();
        }

        public LangElement Yield(Span span, LangElement keyOpt, LangElement valueOpt)
        {
            throw new NotImplementedException();
        }

        public LangElement YieldFrom(Span span, LangElement fromExpr)
        {
            throw new NotImplementedException();
        }

        public LangElement PseudoConstUse(Span span, PseudoConstUse.Types type)
        {
            return PseudoConstUse(span, type);
        }

        public LangElement ExpressionStmt(Span span, LangElement expression)
        {
            return new ExpressionStmt(span, (Expression)expression);
        }

        public LangElement ConstUse(Span span, QualifiedName name, QualifiedName? nameFallback)
        {
            return new GlobalConstUse(span, name, nameFallback);
        }

        public LangElement ClassConstUse(Span span, TypeRef tref, Name name, Span nameSpan)
        {
            return new ClassConstUse(span, tref, name.Value, nameSpan);
        }
    }
}
