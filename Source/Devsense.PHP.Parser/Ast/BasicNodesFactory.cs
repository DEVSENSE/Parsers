using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Nodes factory used by <see cref="Parser.Parser"/>.
    /// </summary>
    public class BasicNodesFactory : INodesFactory<LangElement, Span>, IErrorSink<Span>
    {
        /// <summary>
        /// Gets associated source unit.
        /// </summary>
        public SourceUnit SourceUnit => _sourceUnit;
        readonly SourceUnit _sourceUnit;

        /// <summary>
        /// Gets list of collected errors.
        /// </summary>
        public List<Tuple<Span, ErrorInfo, string[]>> Errors => _errors;
        protected readonly List<Tuple<Span, ErrorInfo, string[]>> _errors = new List<Tuple<Span, ErrorInfo, string[]>>();

        public virtual void Error(Span span, ErrorInfo info, params string[] argsOpt)
        {
            _errors.Add(new Tuple<Span, ErrorInfo, string[]>(span, info, argsOpt));
        }

        List<T> ConvertList<T>(IEnumerable<LangElement> list) where T : LangElement
        {
            Debug.Assert(list.All(s => s == null || s is T), "List of LangELements contains node that is not valid!");
            return list.Cast<T>().ToList();
        }

        public BasicNodesFactory(SourceUnit sourceUnit)
        {
            _sourceUnit = sourceUnit;
        }

        public LangElement ArrayItem(Span span, LangElement expression, LangElement indexOpt)
        {
            return new ItemUse(span, (VarLikeConstructUse)expression, (Expression)indexOpt);
        }
        public Item ArrayItemValue(Span span, LangElement indexOpt, LangElement valueExpr)
        {
            return new ValueItem((Expression)indexOpt, (Expression)valueExpr);
        }
        public Item ArrayItemRef(Span span, LangElement indexOpt, LangElement variable)
        {
            return new RefItem((Expression)indexOpt, (VariableUse)variable);
        }

        public LangElement Assert(Span span, CallSignature signature)
        {
            return new AssertEx(span, signature);
        }

        public LangElement Assignment(Span span, LangElement target, LangElement value, Operations assignOp)
        {
            if (assignOp == Operations.AssignRef)
                return new RefAssignEx(span, (VarLikeConstructUse)target, (Expression)value);
            else
                return new ValueAssignEx(span, assignOp, (VarLikeConstructUse)target, (Expression)value);
        }

        public LangElement BinaryOperation(Span span, Operations operation, LangElement leftExpression, LangElement rightExpression)
        {
            return new BinaryEx(span, operation, (Expression)leftExpression, (Expression)rightExpression);
        }

        public LangElement Block(Span span, IEnumerable<LangElement> statements)
        {
            return new BlockStmt(span, ConvertList<Statement>(statements));
        }

        public LangElement BlockComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public LangElement Call(Span span, LangElement nameExpr, CallSignature signature, TypeRef typeRef)
        {
            Debug.Assert(nameExpr is CompoundVarUse);
            return new IndirectStMtdCall(span, typeRef, (CompoundVarUse)nameExpr, signature.Parameters, signature.GenericParams);
        }

        public LangElement Call(Span span, LangElement nameExpr, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(nameExpr is Expression);
            return new IndirectFcnCall(span, (Expression)nameExpr, signature.Parameters, signature.GenericParams) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }

        public LangElement Call(Span span, Name name, Span nameSpan, CallSignature signature, TypeRef typeRef)
        {
            return new DirectStMtdCall(span, new ClassConstUse(span, typeRef, new VariableNameRef(nameSpan, name.Value)), signature.Parameters, signature.GenericParams);
        }

        public LangElement Call(Span span, QualifiedName name, QualifiedName? nameFallback, Span nameSpan, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(memberOfOpt == null || memberOfOpt is VarLikeConstructUse);
            if (name == QualifiedName.Assert)
                return Assert(span, signature);
            return new DirectFcnCall(span, name, nameFallback, nameSpan, signature.Parameters, signature.GenericParams) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }
        public LangElement ActualParameter(Span span, LangElement expr, ActualParam.Flags flags)
        {
            Debug.Assert(expr != null && expr is Expression);
            return new ActualParam(span, (Expression)expr, flags);
        }

        public LangElement ClassConstDecl(Span span, VariableName name, LangElement initializer)
        {
            Debug.Assert(initializer == null || initializer is Expression);
            return new ClassConstantDecl(span, name.Value, (Expression)initializer);
        }

        public LangElement ColonBlock(Span span, IEnumerable<LangElement> statements, Tokens endToken)
        {
            return Block(span, statements);
        }

        public LangElement Concat(Span span, IEnumerable<LangElement> expressions)
        {
            return new ConcatEx(span, ConvertList<Expression>(expressions));
        }

        public LangElement DeclList(Span span, PhpMemberAttributes attributes, IEnumerable<LangElement> decls)
        {
            Debug.Assert(decls.All(e => e is FieldDecl) || decls.All(e => e is GlobalConstantDecl) || decls.All(e => e is ClassConstantDecl));
            if (decls.All(e => e is GlobalConstantDecl))
                return new GlobalConstDeclList(span, ConvertList<GlobalConstantDecl>(decls), null);
            else if (decls.All(e => e is ClassConstantDecl))
                return new ConstDeclList(span, ConvertList<ClassConstantDecl>(decls), null);
            else //if (decls.All(e => e is FieldDecl))
                return new FieldDeclList(span, attributes, ConvertList<FieldDecl>(decls), null);
        }

        public LangElement Do(Span span, LangElement body, LangElement cond)
        {
            return new WhileStmt(span, WhileStmt.Type.Do, (Expression)cond, (Statement)body);
        }

        public LangElement Echo(Span span, IEnumerable<LangElement> parameters)
        {
            return new EchoStmt(span, ConvertList<Expression>(parameters));
        }

        public LangElement Unset(Span span, IEnumerable<LangElement> variables)
        {
            return new UnsetStmt(span, ConvertList<VariableUse>(variables));
        }

        public LangElement Eval(Span span, LangElement code)
        {
            return new EvalEx(span, (Expression)code);
        }
        public LangElement ParenthesisExpression(Span span, LangElement expression)
        {
            return expression;
        }

        public LangElement Exit(Span span, LangElement statusOpt)
        {
            return new ExitEx(span, (Expression)statusOpt);
        }

        public LangElement Empty(Span span, LangElement code)
        {
            return new EmptyEx(span, (Expression)code);
        }

        public LangElement Isset(Span span, IEnumerable<LangElement> variables)
        {
            return new IssetEx(span, ConvertList<VariableUse>(variables));
        }

        public LangElement FieldDecl(Span span, VariableName name, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new FieldDecl(span, name.Value, (Expression)initializerOpt);
        }

        public LangElement For(Span span, IEnumerable<LangElement> init, IEnumerable<LangElement> cond, IEnumerable<LangElement> action, LangElement body)
        {
            return new ForStmt(span, ConvertList<Expression>(init), ConvertList<Expression>(cond), ConvertList<Expression>(action), (Statement)body);
        }

        public LangElement Foreach(Span span, LangElement enumeree, VariableUse keyOpt, VariableUse value, LangElement body)
        {
            return new ForeachStmt(span, (Expression)enumeree, new ForeachVar(keyOpt, false), new ForeachVar(value, false), (Statement)body);
        }

        public LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, 
            Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, LangElement body)
        {
            Debug.Assert(body is BlockStmt || body is Statement);
            return new FunctionDecl(_sourceUnit, span, conditional, new Scope(), attributes, new NameRef(nameSpan, name.Value), null, aliasReturn,
                formalParams.ToList(), formalParamsSpan, (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                (BlockStmt)body, null, returnType);
        }

        public LangElement Lambda(Span span, Span headingSpan, bool aliasReturn, TypeRef returnType, IEnumerable<FormalParam> formalParams, 
            Span formalParamsSpan, IEnumerable<FormalParam> lexicalVars, LangElement body)
        {
            Debug.Assert(body is BlockStmt || body is Statement);
            return new LambdaFunctionExpr(_sourceUnit, span, headingSpan,
                new Scope(), null, false, formalParams.ToList(), formalParamsSpan, lexicalVars.ToList(),
                (BlockStmt)body, returnType);
        }

        public LangElement Parameter(Span span, string name, TypeRef typeOpt, FormalParam.Flags flags, Expression initValue)
        {
            return new FormalParam(span, name, typeOpt, flags, initValue, null);
        }

        public LangElement GlobalCode(Span span, IEnumerable<LangElement> statements, NamingContext context)
        {
            _sourceUnit.Naming = context;
            return new GlobalCode(span, ConvertList<Statement>(statements), _sourceUnit);
        }

        public LangElement GlobalConstDecl(Span span, bool conditional, VariableName name, LangElement initializer)
        {
            return new GlobalConstantDecl(_sourceUnit, span, conditional, name.Value, (Expression)initializer);
        }

        public LangElement Goto(Span span, string label, Span labelSpan)
        {
            return new GotoStmt(span, new VariableNameRef(labelSpan, label));
        }

        public LangElement HaltCompiler(Span span)
        {
            return new HaltCompiler(span);
        }

        public LangElement If(Span span, LangElement cond, LangElement body, LangElement elseOpt)
        {
            List<ConditionalStmt> conditions = new List<ConditionalStmt>() { new ConditionalStmt(span, (Expression)cond, (Statement)body) };
            if (elseOpt != null)
            {
                Debug.Assert(elseOpt is IfStmt);
                IfStmt elseIf = (IfStmt)elseOpt;
                conditions = conditions.Concat(elseIf.Conditions).ToList();
            }
            return new IfStmt(span, conditions);
        }

        public LangElement Inclusion(Span span, bool conditional, InclusionTypes type, LangElement fileNameExpression)
        {
            return new IncludingEx(_sourceUnit, conditional, span, type, (Expression)fileNameExpression);
        }

        public LangElement IncrementDecrement(Span span, LangElement refexpression, bool inc, bool post)
        {
            return new IncDecEx(span, inc, post, (VariableUse)refexpression);
        }

        public LangElement InlineHtml(Span span, string html)
        {
            return new EchoStmt(span, html);
        }

        public LangElement InstanceOf(Span span, LangElement expression, TypeRef typeRef)
        {
            return new InstanceOfEx(span, (Expression)expression, typeRef);
        }

        public LangElement Jump(Span span, JumpStmt.Types type, LangElement exprOpt)
        {
            return new JumpStmt(span, type, (Expression)exprOpt);
        }

        public LangElement Label(Span span, string label, Span labelSpan)
        {
            return new LabelStmt(span, new VariableNameRef(labelSpan, label));
        }

        public LangElement LineComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public LangElement List(Span span, IEnumerable<Item> targets)
        {
            return new ListEx(span, targets.ToList());
        }

        public LangElement Literal(Span span, object value)
        {
            if (value is long)
                return new LongIntLiteral(span, (long)value);
            else if(value is int)
                return new LongIntLiteral(span, (int)value);
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
            throw new ArgumentException("Value does not have supported type.");
        }

        public LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, IEnumerable<LangElement> statements, NamingContext context)
        {
            NamespaceDecl space = new NamespaceDecl(span, name.HasValue ? new QualifiedNameRef(nameSpan, name.Value) : QualifiedNameRef.Invalid, true);
            space.Naming = context;
            space.Statements = (statements != null) ? ConvertList<Statement>(statements) : null;
            return space;
        }

        public LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, LangElement block, NamingContext context)
        {
            Debug.Assert(block != null);
            NamespaceDecl space = new NamespaceDecl(span, name.HasValue ? new QualifiedNameRef(nameSpan, name.Value) : QualifiedNameRef.Invalid, false);
            space.Naming = context;
            space.Statements = new List<Statement>() { (Statement)block };
            return space;
        }

        public LangElement Declare(Span span, LangElement statementOpt)
        {
            Debug.Assert(statementOpt == null || statementOpt is Statement);
            return new DeclareStmt(span, (Statement)statementOpt);
        }

        public LangElement New(Span span, TypeRef classNameRef, IEnumerable<ActualParam> argsOpt)
        {
            return new NewEx(span, classNameRef, argsOpt.ToList());
        }

        public LangElement NewArray(Span span, IEnumerable<Item> itemsOpt)
        {
            return new ArrayEx(span, itemsOpt.ToList());
        }

        public LangElement PHPDoc(Span span, string content)
        {
            return new PHPDocBlock(content, span);
        }

        public LangElement Shell(Span span, LangElement command)
        {
            Debug.Assert(command is Expression);
            return new ShellEx(span, (Expression)command);
        }

        public LangElement Switch(Span span, LangElement value, List<LangElement> block)
        {
            return new SwitchStmt(span, (Expression)value, ConvertList<SwitchItem>(block));
        }

        public LangElement Case(Span span, LangElement valueOpt, LangElement block)
        {
            Debug.Assert(block is BlockStmt);
            if (valueOpt != null)
                return new CaseItem(span, (Expression)valueOpt, ((BlockStmt)block).Statements);
            else
                return new DefaultItem(span, ((BlockStmt)block).Statements);
        }

        public LangElement TraitUse(Span span, IEnumerable<QualifiedName> traits, IEnumerable<LangElement> adaptations)
        {
            Debug.Assert(traits != null);
            return new TraitsUse(span, 0, traits.ToList(), (adaptations != null) ? ConvertList<TraitsUse.TraitAdaptation>(adaptations) : null);
        }

        public LangElement TraitAdaptationPrecedence(Span span, Tuple<QualifiedName?, Name> name, IEnumerable<QualifiedName> precedences)
        {
            Debug.Assert(precedences != null);
            return new TraitsUse.TraitAdaptationPrecedence(span, name, precedences.ToList());
        }

        public LangElement TraitAdaptationAlias(Span span, Tuple<QualifiedName?, Name> name, string identifierOpt, PhpMemberAttributes? attributeOpt)
        {
            Debug.Assert(!string.IsNullOrEmpty(identifierOpt) || attributeOpt != null);
            return new TraitsUse.TraitAdaptationAlias(span, name, identifierOpt, attributeOpt);
        }

        public LangElement Global(Span span, List<LangElement> variables)
        {
            return new GlobalStmt(span, ConvertList<SimpleVarUse>(variables));
        }

        public LangElement TryCatch(Span span, LangElement body, IEnumerable<CatchItem> catches, LangElement finallyBlockOpt)
        {
            Debug.Assert(body is BlockStmt);
            return new TryStmt(span, ((BlockStmt)body).Statements, catches.ToList(), (FinallyItem)finallyBlockOpt);
        }

        public LangElement Catch(Span span, TypeRef typeOpt, DirectVarUse variable, LangElement block)
        {
            Debug.Assert(block is BlockStmt && typeOpt != null);
            return new CatchItem(span, typeOpt, variable, ((BlockStmt)block).Statements);
        }

        public LangElement Finally(Span span, LangElement block)
        {
            Debug.Assert(block is BlockStmt);
            return new FinallyItem(span, ((BlockStmt)block).Statements);
        }

        public LangElement Throw(Span span, LangElement expression)
        {
            Debug.Assert(expression is Expression);
            return new ThrowStmt(span, (Expression)expression);
        }

        public LangElement Type(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, TypeRef baseClassOpt, IEnumerable<TypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            if (implements == null) implements = TypeRef.EmptyList;

            Debug.Assert(members != null && implements != null);
            return new NamedTypeDecl(_sourceUnit, span, headingSpan, conditional, new Scope(), attributes, false,
                new NameRef(nameSpan, name), null, (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                QualifiedNameRef.FromTypeRef(baseClassOpt), implements.Select(QualifiedNameRef.FromTypeRef).ToList(),
                ConvertList<TypeMemberDecl>(members), bodySpan, null);
        }

        public LangElement AnonymousTypeReference(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, IEnumerable<FormalTypeParam> typeParamsOpt, TypeRef baseClassOpt, IEnumerable<TypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            if (implements == null) implements = TypeRef.EmptyList;

            Debug.Assert(members != null && implements != null);
            return new AnonymousTypeRef(span, new AnonymousTypeDecl(_sourceUnit, span, headingSpan,
                conditional, new Scope(), attributes, false, null, (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                QualifiedNameRef.FromTypeRef(baseClassOpt), implements.Select(QualifiedNameRef.FromTypeRef).ToList(), ConvertList<TypeMemberDecl>(members), bodySpan, null));
        }

        public LangElement Method(Span span, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Span returnTypeSpan, string name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, IEnumerable<ActualParam> baseCtorParams, LangElement body)
        {
            Debug.Assert(body is BlockStmt || body is Statement);

            return new MethodDecl(span, formalParamsSpan.End, body.Span.Start, new NameRef(nameSpan, name), aliasReturn, formalParams.ToList(),
                formalParamsSpan, (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                (BlockStmt)body, attributes, (baseCtorParams != null) ? baseCtorParams.ToList() : new List<ActualParam>(), null, returnType);
        }

        public LangElement UnaryOperation(Span span, Operations operation, LangElement expression)
        {
            Debug.Assert(expression is Expression);
            return new UnaryEx(operation, (Expression)expression);
        }

        public LangElement Variable(Span span, LangElement nameExpr, TypeRef typeRef)
        {
            Debug.Assert(typeRef != null);
            return new IndirectStFldUse(span, typeRef, (Expression)nameExpr);
        }

        public LangElement Variable(Span span, LangElement nameExpr, LangElement memberOfOpt)
        {
            return new IndirectVarUse(span, 1, (Expression)nameExpr) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }

        public LangElement Variable(Span span, VariableName name, TypeRef typeRef)
        {
            Debug.Assert(typeRef != null);
            return new DirectStFldUse(span, typeRef, name, new Span(span.End - name.Value.Length, name.Value.Length));
        }

        public LangElement Variable(Span span, VariableName name, LangElement memberOfOpt)
        {
            return new DirectVarUse(span, name) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }
        public LangElement TypeReference(Span span, QualifiedName className, bool isNullable, List<TypeRef> genericParamsOpt)
        {
            TypeRef type = null;
            if (className == QualifiedName.Boolean || className == QualifiedName.Integer ||
                className == QualifiedName.Float || className == QualifiedName.String ||
                className == QualifiedName.Null || className == QualifiedName.Resource)
                type = new PrimitiveTypeRef(span, new PrimitiveTypeName(className));
            else
                type = new DirectTypeRef(span, className);
            if (isNullable)
                type = new NullableTypeRef(span, type);
            if (genericParamsOpt != null && genericParamsOpt != GenericTypeRef.EmptyList)
                type = new GenericTypeRef(span, type, genericParamsOpt);
            return type;
        }
        public LangElement TypeReference(Span span, LangElement varName, List<TypeRef> genericParamsOpt)
        {
            TypeRef type = new IndirectTypeRef(span, (VariableUse)varName);
            if (genericParamsOpt != null && genericParamsOpt != GenericTypeRef.EmptyList)
                type = new GenericTypeRef(span, type, genericParamsOpt);
            return type;
        }
        public LangElement TypeReference(Span span, IEnumerable<LangElement> classes, List<TypeRef> genericParamsOpt)
        {
            Debug.Assert(classes != null && classes.Count() > 0 && classes.All(c => c is TypeRef));
            if (classes.Count() == 1)
                return classes.First();

            TypeRef type = null;
            type = new MultipleTypeRef(span, ConvertList<TypeRef>(classes));
            if (genericParamsOpt != null && genericParamsOpt != GenericTypeRef.EmptyList)
                type = new GenericTypeRef(span, type, genericParamsOpt);
            return type;
        }

        public LangElement While(Span span, LangElement cond, LangElement body)
        {
            Debug.Assert(cond is Expression && body is Statement);
            return new WhileStmt(span, WhileStmt.Type.While, (Expression)cond, (Statement)body);
        }

        public LangElement Yield(Span span, LangElement keyOpt, LangElement valueOpt)
        {
            return new YieldEx(span, (Expression)keyOpt, (Expression)valueOpt);
        }

        public LangElement YieldFrom(Span span, LangElement fromExpr)
        {
            return new YieldFromEx(span, (Expression)fromExpr);
        }

        public LangElement PseudoConstUse(Span span, PseudoConstUse.Types type)
        {
            return PseudoConstUse(span, type);
        }

        public LangElement ExpressionStmt(Span span, LangElement expression)
        {
            Debug.Assert(expression is Expression);
            return new ExpressionStmt(span, (Expression)expression);
        }
        public LangElement Static(Span span, IEnumerable<LangElement> staticVariables)
        {
            return new StaticStmt(span, ConvertList<StaticVarDecl>(staticVariables));
        }
        public LangElement StaticVarDecl(Span span, VariableName name, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new StaticVarDecl(span, new DirectVarUse(span, name), (Expression)initializerOpt);
        }

        public LangElement ConstUse(Span span, QualifiedName name, QualifiedName? nameFallback)
        {
            return new GlobalConstUse(span, name, nameFallback);
        }

        public LangElement ClassConstUse(Span span, TypeRef tref, Name name, Span nameSpan)
        {
            return new ClassConstUse(span, tref, new VariableNameRef(nameSpan, name.Value));
        }

        public LangElement ConditionalEx(Span span, LangElement condExpr, LangElement trueExpr, LangElement falseExpr)
        {
            Debug.Assert(condExpr is Expression && trueExpr is Expression && falseExpr is Expression);
            return new ConditionalEx(span, (Expression)condExpr, (Expression)trueExpr, (Expression)falseExpr);
        }
    }
}
