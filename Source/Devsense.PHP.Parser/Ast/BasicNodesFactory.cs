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
    public class BasicNodesFactory : INodesFactory<LangElement, Span>
    {
        /// <summary>
        /// Gets associated source unit.
        /// </summary>
        public SourceUnit SourceUnit => _sourceUnit;
        readonly SourceUnit _sourceUnit;

        List<T> ConvertList<T>(IEnumerable<LangElement> list) where T : LangElement
        {
            if (list == null) return null;
            Debug.Assert(list.All(s => s == null || s is T), "List of LangELements contains node that is not valid!");
            return list.Cast<T>().ToList();
        }

        public BasicNodesFactory(SourceUnit sourceUnit)
        {
            _sourceUnit = sourceUnit;
        }

        public virtual LangElement ArrayItem(Span span, LangElement expression, LangElement indexOpt)
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

        public virtual LangElement Assignment(Span span, LangElement target, LangElement value, Operations assignOp)
        {
            if (assignOp == Operations.AssignRef)
                return new RefAssignEx(span, (VarLikeConstructUse)target, (Expression)value);
            else
                return new ValueAssignEx(span, assignOp, (VarLikeConstructUse)target, (Expression)value);
        }

        public virtual LangElement BinaryOperation(Span span, Operations operation, LangElement leftExpression, LangElement rightExpression)
        {
            return new BinaryEx(span, operation, (Expression)leftExpression, (Expression)rightExpression);
        }

        public virtual LangElement Block(Span span, IEnumerable<LangElement> statements)
        {
            return new BlockStmt(span, ConvertList<Statement>(statements));
        }

        public virtual LangElement BlockComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public virtual LangElement Call(Span span, LangElement nameExpr, CallSignature signature, TypeRef typeRef)
        {
            Debug.Assert(nameExpr is CompoundVarUse);
            return new IndirectStMtdCall(span, typeRef, (CompoundVarUse)nameExpr, signature.Parameters, signature.GenericParams);
        }

        public virtual LangElement Call(Span span, LangElement nameExpr, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(nameExpr is Expression);
            return new IndirectFcnCall(span, (Expression)nameExpr, signature.Parameters, signature.GenericParams) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }

        public virtual LangElement Call(Span span, Name name, Span nameSpan, CallSignature signature, TypeRef typeRef)
        {
            return new DirectStMtdCall(span, new ClassConstUse(span, typeRef, new VariableNameRef(nameSpan, name.Value)), signature.Parameters, signature.GenericParams);
        }

        public virtual LangElement Call(Span span, QualifiedName name, QualifiedName? nameFallback, Span nameSpan, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(memberOfOpt == null || memberOfOpt is VarLikeConstructUse);
            return new DirectFcnCall(span, name, nameFallback, nameSpan, signature.Parameters, signature.GenericParams) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }
        public virtual LangElement ActualParameter(Span span, LangElement expr, ActualParam.Flags flags)
        {
            Debug.Assert(expr != null && expr is Expression);
            return new ActualParam(span, (Expression)expr, flags);
        }

        public virtual LangElement ClassConstDecl(Span span, VariableName name, Span nameSpan, LangElement initializer)
        {
            Debug.Assert(initializer == null || initializer is Expression);
            return new ClassConstantDecl(span, name.Value, nameSpan, (Expression)initializer);
        }

        public virtual LangElement ColonBlock(Span span, IEnumerable<LangElement> statements, Tokens endToken)
        {
            return Block(span, statements);
        }
        public virtual LangElement SimpleBlock(Span span, IEnumerable<LangElement> statements)
        {
            return Block(span, statements);
        }

        public virtual LangElement Concat(Span span, IEnumerable<LangElement> expressions)
        {
            return new ConcatEx(span, ConvertList<Expression>(expressions));
        }

        public virtual LangElement DeclList(Span span, PhpMemberAttributes attributes, IEnumerable<LangElement> decls)
        {
            Debug.Assert(decls.All(e => e is FieldDecl) || decls.All(e => e is GlobalConstantDecl) || decls.All(e => e is ClassConstantDecl));
            if (decls.All(e => e is GlobalConstantDecl))
                return new GlobalConstDeclList(span, ConvertList<GlobalConstantDecl>(decls), null);
            else if (decls.All(e => e is ClassConstantDecl))
                return new ConstDeclList(span, ConvertList<ClassConstantDecl>(decls), null);
            else //if (decls.All(e => e is FieldDecl))
                return new FieldDeclList(span, attributes, ConvertList<FieldDecl>(decls), null);
        }

        public virtual LangElement Do(Span span, LangElement body, LangElement cond)
        {
            return new WhileStmt(span, WhileStmt.Type.Do, (Expression)cond, (Statement)body);
        }

        public virtual LangElement Echo(Span span, IEnumerable<LangElement> parameters)
        {
            return new EchoStmt(span, ConvertList<Expression>(parameters));
        }

        public virtual LangElement Unset(Span span, IEnumerable<LangElement> variables)
        {
            return new UnsetStmt(span, ConvertList<VariableUse>(variables));
        }

        public virtual LangElement Eval(Span span, LangElement code)
        {
            return new EvalEx(span, (Expression)code);
        }
        public virtual LangElement ParenthesisExpression(Span span, LangElement expression)
        {
            return expression;
        }

        public virtual LangElement Exit(Span span, LangElement statusOpt)
        {
            return new ExitEx(span, (Expression)statusOpt);
        }

        public virtual LangElement Empty(Span span, LangElement code)
        {
            return new EmptyEx(span, (Expression)code);
        }

        /// <summary>
        /// An empty statement (<c>;</c>).
        /// </summary>
        /// <param name="span">Semicolon position.</param>
        /// <returns>Empty statement.</returns>
        public virtual LangElement EmptyStmt(Span span) => new EmptyStmt(span);

        public virtual LangElement Isset(Span span, IEnumerable<LangElement> variables)
        {
            return new IssetEx(span, ConvertList<VariableUse>(variables));
        }

        public virtual LangElement FieldDecl(Span span, VariableName name, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new FieldDecl(span, name.Value, (Expression)initializerOpt);
        }

        public virtual LangElement For(Span span, IEnumerable<LangElement> init, IEnumerable<LangElement> cond, IEnumerable<LangElement> action, LangElement body)
        {
            return new ForStmt(span, ConvertList<Expression>(init), ConvertList<Expression>(cond), ConvertList<Expression>(action), (Statement)body);
        }

        public virtual LangElement Foreach(Span span, LangElement enumeree, LangElement keyOpt, LangElement value, LangElement body)
        {
            ForeachVar key = keyOpt == null? null: (keyOpt is VariableUse ? new ForeachVar((VariableUse)keyOpt, false) : new ForeachVar((ListEx)keyOpt));
            ForeachVar val = value is VariableUse ? new ForeachVar((VariableUse)value, false) : new ForeachVar((ListEx)value);
            return new ForeachStmt(span, (Expression)enumeree, key, val, (Statement)body);
        }

        public virtual LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, 
            Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, LangElement body)
        {
            return new FunctionDecl(span, conditional, attributes, new NameRef(nameSpan, name.Value), aliasReturn,
                formalParams.ToList(), formalParamsSpan, (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                (BlockStmt)body, null, returnType);
        }

        public virtual LangElement Lambda(Span span, Span headingSpan, bool aliasReturn, TypeRef returnType, IEnumerable<FormalParam> formalParams, 
            Span formalParamsSpan, IEnumerable<FormalParam> lexicalVars, LangElement body)
        {
            return new LambdaFunctionExpr(span, headingSpan,
                new Scope(), false, formalParams.ToList(), formalParamsSpan, lexicalVars.ToList(),
                (BlockStmt)body, returnType);
        }

        public virtual LangElement Parameter(Span span, string name, Span nameSpan, TypeRef typeOpt, FormalParam.Flags flags, Expression initValue)
        {
            return new FormalParam(span, name, nameSpan, typeOpt, flags, initValue, null);
        }

        public virtual LangElement GlobalCode(Span span, IEnumerable<LangElement> statements, NamingContext context)
        {
            _sourceUnit.Naming = context;
            var ast =  new GlobalCode(span, ConvertList<Statement>(statements), _sourceUnit);

            // link to parent nodes
            UpdateParentVisitor.UpdateParents(ast);

            //
            return ast;
        }

        public virtual LangElement GlobalConstDecl(Span span, bool conditional, VariableName name, Span nameSpan, LangElement initializer)
        {
            return new GlobalConstantDecl(span, conditional, name.Value, nameSpan, (Expression)initializer);
        }

        public virtual LangElement Goto(Span span, string label, Span labelSpan)
        {
            return new GotoStmt(span, new VariableNameRef(labelSpan, label));
        }

        public virtual LangElement HaltCompiler(Span span)
        {
            return new HaltCompiler(span);
        }

        public virtual LangElement If(Span span, LangElement cond, LangElement body, LangElement elseOpt)
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

        public virtual LangElement Inclusion(Span span, bool conditional, InclusionTypes type, LangElement fileNameExpression)
        {
            return new IncludingEx(conditional, span, type, (Expression)fileNameExpression);
        }

        public virtual LangElement IncrementDecrement(Span span, LangElement refexpression, bool inc, bool post)
        {
            return new IncDecEx(span, inc, post, (VariableUse)refexpression);
        }

        public virtual LangElement InlineHtml(Span span, string html)
        {
            return new EchoStmt(span, html);
        }

        public virtual LangElement InstanceOf(Span span, LangElement expression, TypeRef typeRef)
        {
            return new InstanceOfEx(span, (Expression)expression, typeRef);
        }

        public virtual LangElement Jump(Span span, JumpStmt.Types type, LangElement exprOpt)
        {
            return new JumpStmt(span, type, (Expression)exprOpt);
        }

        public virtual LangElement Label(Span span, string label, Span labelSpan)
        {
            return new LabelStmt(span, new VariableNameRef(labelSpan, label));
        }

        public virtual LangElement LineComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public virtual LangElement List(Span span, IEnumerable<Item> targets)
        {
            return new ListEx(span, targets.ToList());
        }

        public virtual LangElement Literal(Span span, object value)
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

        public virtual LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, NamingContext context)
        {
            NamespaceDecl space = new NamespaceDecl(span, name.HasValue ? new QualifiedNameRef(nameSpan, name.Value) : QualifiedNameRef.Invalid, true);
            space.Naming = context;
            return space;
        }

        public virtual LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, LangElement block, NamingContext context)
        {
            Debug.Assert(block != null);
            NamespaceDecl space = new NamespaceDecl(span, name.HasValue ? new QualifiedNameRef(nameSpan, name.Value) : QualifiedNameRef.Invalid, false);
            space.Naming = context;
            space.Body = (BlockStmt)block;
            return space;
        }

        public virtual LangElement Declare(Span span, LangElement statementOpt)
        {
            Debug.Assert(statementOpt == null || statementOpt is Statement);
            return new DeclareStmt(span, (Statement)statementOpt);
        }

        public virtual LangElement New(Span span, TypeRef classNameRef, IEnumerable<ActualParam> argsOpt)
        {
            return new NewEx(span, classNameRef, argsOpt.ToList());
        }

        public virtual LangElement NewArray(Span span, IEnumerable<Item> itemsOpt)
        {
            return new ArrayEx(span, itemsOpt.ToList());
        }

        public virtual LangElement PHPDoc(Span span, LangElement block)
        {
            throw new NotImplementedException();
        }

        public virtual LangElement Shell(Span span, LangElement command)
        {
            Debug.Assert(command is Expression);
            return new ShellEx(span, (Expression)command);
        }

        public virtual LangElement Switch(Span span, LangElement value, List<LangElement> block)
        {
            return new SwitchStmt(span, (Expression)value, ConvertList<SwitchItem>(block));
        }

        public virtual LangElement Case(Span span, LangElement valueOpt, LangElement block)
        {
            Debug.Assert(block is BlockStmt);
            if (valueOpt != null)
                return new CaseItem(span, (Expression)valueOpt, ((BlockStmt)block).Statements);
            else
                return new DefaultItem(span, ((BlockStmt)block).Statements);
        }

        public virtual LangElement TraitUse(Span span, IEnumerable<QualifiedNameRef> traits, IEnumerable<LangElement> adaptations)
        {
            Debug.Assert(traits != null);
            return new TraitsUse(span, 0, traits.ToList(), (adaptations != null) ? ConvertList<TraitsUse.TraitAdaptation>(adaptations) : null);
        }

        public virtual LangElement TraitAdaptationPrecedence(Span span, Tuple<QualifiedNameRef?, NameRef> name, IEnumerable<QualifiedNameRef> precedences)
        {
            Debug.Assert(precedences != null);
            return new TraitsUse.TraitAdaptationPrecedence(span, name, precedences.ToList());
        }

        public virtual LangElement TraitAdaptationAlias(Span span, Tuple<QualifiedNameRef?, NameRef> name, NameRef? identifierOpt, PhpMemberAttributes? attributeOpt)
        {
            Debug.Assert(identifierOpt != null || attributeOpt != null);
            return new TraitsUse.TraitAdaptationAlias(span, name, identifierOpt, attributeOpt);
        }

        public virtual LangElement Global(Span span, List<LangElement> variables)
        {
            return new GlobalStmt(span, ConvertList<SimpleVarUse>(variables));
        }

        public virtual LangElement TryCatch(Span span, LangElement body, IEnumerable<CatchItem> catches, LangElement finallyBlockOpt)
        {
            Debug.Assert(body is BlockStmt);
            return new TryStmt(span, (BlockStmt)body, catches.ToList(), (FinallyItem)finallyBlockOpt);
        }

        public virtual LangElement Catch(Span span, TypeRef typeOpt, DirectVarUse variable, LangElement block)
        {
            Debug.Assert(block is BlockStmt && typeOpt != null);
            return new CatchItem(span, typeOpt, variable, (BlockStmt)block);
        }

        public virtual LangElement Finally(Span span, LangElement block)
        {
            Debug.Assert(block is BlockStmt);
            return new FinallyItem(span, (BlockStmt)block);
        }

        public virtual LangElement Throw(Span span, LangElement expression)
        {
            Debug.Assert(expression is Expression);
            return new ThrowStmt(span, (Expression)expression);
        }

        public virtual LangElement Type(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, TypeRef baseClassOpt, IEnumerable<TypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            if (implements == null) implements = TypeRef.EmptyList;

            Debug.Assert(members != null && implements != null);
            return new NamedTypeDecl(span, headingSpan, conditional, new Scope(), attributes, false,
                new NameRef(nameSpan, name), (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                QualifiedNameRef.FromTypeRef(baseClassOpt), implements.Select(QualifiedNameRef.FromTypeRef).ToList(),
                ConvertList<TypeMemberDecl>(members), bodySpan, null);
        }

        public virtual LangElement AnonymousTypeReference(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, IEnumerable<FormalTypeParam> typeParamsOpt, TypeRef baseClassOpt, IEnumerable<TypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            if (implements == null) implements = TypeRef.EmptyList;

            Debug.Assert(members != null && implements != null);
            return new AnonymousTypeRef(span, new AnonymousTypeDecl(span, headingSpan,
                conditional, new Scope(), attributes, false, (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                QualifiedNameRef.FromTypeRef(baseClassOpt), implements.Select(QualifiedNameRef.FromTypeRef).ToList(), ConvertList<TypeMemberDecl>(members), bodySpan, null));
        }

        public virtual LangElement Method(Span span, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Span returnTypeSpan, string name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, IEnumerable<ActualParam> baseCtorParams, LangElement body)
        {
            return new MethodDecl(span, new NameRef(nameSpan, name), aliasReturn, formalParams.ToList(),
                formalParamsSpan, (typeParamsOpt != null) ? typeParamsOpt.ToList() : FormalTypeParam.EmptyList,
                (BlockStmt)body, attributes, (baseCtorParams != null) ? baseCtorParams.ToList() : new List<ActualParam>(), null, returnType);
        }

        public virtual LangElement UnaryOperation(Span span, Operations operation, LangElement expression)
        {
            Debug.Assert(expression is Expression);
            return new UnaryEx(span, operation, (Expression)expression);
        }

        public virtual LangElement Variable(Span span, LangElement nameExpr, TypeRef typeRef)
        {
            Debug.Assert(typeRef != null);
            return new IndirectStFldUse(span, typeRef, (Expression)nameExpr);
        }

        public virtual LangElement Variable(Span span, LangElement nameExpr, LangElement memberOfOpt)
        {
            return new IndirectVarUse(span, 1, (Expression)nameExpr) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }

        public virtual LangElement Variable(Span span, VariableName name, TypeRef typeRef)
        {
            Debug.Assert(typeRef != null);
            return new DirectStFldUse(span, typeRef, name, new Span(span.End - name.Value.Length - 1, name.Value.Length + 1));
        }

        public virtual LangElement Variable(Span span, VariableName name, LangElement memberOfOpt)
        {
            return new DirectVarUse(new Span(span.End - name.Value.Length - 1, name.Value.Length + 1), name) { IsMemberOf = (VarLikeConstructUse)memberOfOpt };
        }
        public virtual LangElement TypeReference(Span span, QualifiedName className, bool isNullable, List<TypeRef> genericParamsOpt)
        {
            TypeRef type = null;
            if (className.IsPrimitiveTypeName)
                type = new PrimitiveTypeRef(span, new PrimitiveTypeName(className));
            else
                type = new DirectTypeRef(span, className);
            if (isNullable)
                type = new NullableTypeRef(span, type);
            if (genericParamsOpt != null && genericParamsOpt != GenericTypeRef.EmptyList)
                type = new GenericTypeRef(span, type, genericParamsOpt);
            return type;
        }
        public virtual LangElement TypeReference(Span span, LangElement varName, List<TypeRef> genericParamsOpt)
        {
            TypeRef type = new IndirectTypeRef(span, (VariableUse)varName);
            if (genericParamsOpt != null && genericParamsOpt != GenericTypeRef.EmptyList)
                type = new GenericTypeRef(span, type, genericParamsOpt);
            return type;
        }
        public virtual LangElement TypeReference(Span span, IEnumerable<LangElement> classes, List<TypeRef> genericParamsOpt)
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

        public virtual LangElement While(Span span, LangElement cond, LangElement body)
        {
            Debug.Assert(cond is Expression && body is Statement);
            return new WhileStmt(span, WhileStmt.Type.While, (Expression)cond, (Statement)body);
        }

        public virtual LangElement Yield(Span span, LangElement keyOpt, LangElement valueOpt)
        {
            return new YieldEx(span, (Expression)keyOpt, (Expression)valueOpt);
        }

        public virtual LangElement YieldFrom(Span span, LangElement fromExpr)
        {
            return new YieldFromEx(span, (Expression)fromExpr);
        }

        public virtual LangElement PseudoConstUse(Span span, PseudoConstUse.Types type)
        {
            return new PseudoConstUse(span, type);
        }

        public virtual LangElement ExpressionStmt(Span span, LangElement expression)
        {
            Debug.Assert(expression is Expression);
            return new ExpressionStmt(span, (Expression)expression);
        }
        public virtual LangElement Static(Span span, IEnumerable<LangElement> staticVariables)
        {
            return new StaticStmt(span, ConvertList<StaticVarDecl>(staticVariables));
        }
        public virtual LangElement StaticVarDecl(Span span, VariableName name, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new StaticVarDecl(span, new DirectVarUse(span, name), (Expression)initializerOpt);
        }

        public virtual LangElement ConstUse(Span span, QualifiedName name, QualifiedName? nameFallback)
        {
            return new GlobalConstUse(span, name, nameFallback);
        }

        public virtual LangElement ClassConstUse(Span span, TypeRef tref, Name name, Span nameSpan)
        {
            return new ClassConstUse(span, tref, new VariableNameRef(nameSpan, name.Value));
        }

        public virtual LangElement ConditionalEx(Span span, LangElement condExpr, LangElement trueExpr, LangElement falseExpr)
        {
            return new ConditionalEx(span, (Expression)condExpr, (Expression)trueExpr, (Expression)falseExpr);
        }
    }
}
