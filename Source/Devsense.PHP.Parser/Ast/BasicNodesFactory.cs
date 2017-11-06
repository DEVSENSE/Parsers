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

        private static bool IsNull<T>(T obj) => obj == null;

        public BasicNodesFactory(SourceUnit sourceUnit)
        {
            _sourceUnit = sourceUnit;
        }

        public virtual LangElement ArrayItem(Span span, bool braces, LangElement expression, LangElement indexOpt)
        {
            return new ItemUse(span, (Expression)expression, (Expression)indexOpt, isBraces: braces);
        }
        public virtual Item ArrayItemValue(Span span, LangElement indexOpt, LangElement valueExpr)
        {
            return new ValueItem((Expression)indexOpt, (Expression)valueExpr);
        }
        public virtual Item ArrayItemRef(Span span, LangElement indexOpt, LangElement variable)
        {
            return new RefItem((Expression)indexOpt, (VariableUse)variable);
        }

        public virtual LangElement Assignment(Span span, LangElement target, LangElement value, Operations assignOp, Span operationSpan, Span refSpan)
        {
            if (assignOp == Operations.AssignRef)
            {
                return new RefAssignEx(span, (VarLikeConstructUse)target, (Expression)value)
                { OperatorPosition = operationSpan.Start, ReferencePosition = refSpan.Start };
            }
            else
            {
                return new ValueAssignEx(span, assignOp, (VarLikeConstructUse)target, (Expression)value)
                { OperatorPosition = operationSpan.Start };
            }
        }

        public virtual LangElement BinaryOperation(Span span, Operations operation, LangElement leftExpression, LangElement rightExpression)
        {
            return new BinaryEx(span, operation, (Expression)leftExpression, (Expression)rightExpression);
        }

        public virtual LangElement Block(Span span, IEnumerable<LangElement> statements)
        {
            return new BlockStmt(BlockSpan(span, statements), statements.CastToArray<Statement>());
        }

        private static Span BlockSpan(Span span, IEnumerable<LangElement> statements)
        {
            return (span.IsValid || !statements.Any())
                ? span
                : Span.FromBounds(statements.First().Span.Start, statements.Last().Span.End);
        }

        public virtual LangElement TraitAdaptationBlock(Span span, IEnumerable<LangElement> adaptations)
        {
            return new TraitAdaptationBlock(span, adaptations.CastToArray<TraitsUse.TraitAdaptation>());
        }

        public virtual LangElement BlockComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public virtual LangElement Call(Span span, LangElement nameExpr, CallSignature signature, TypeRef typeRef)
        {
            Debug.Assert(nameExpr is Expression);
            var call = new IndirectStMtdCall(span, typeRef, (Expression)nameExpr, signature.Parameters, signature.GenericParams);
            call.CallSignature.Position = signature.Position;
            return call;
        }

        public virtual LangElement Call(Span span, LangElement nameExpr, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(nameExpr is Expression);
            var call = new IndirectFcnCall(span, (Expression)nameExpr, signature.Parameters, signature.GenericParams) { IsMemberOf = (Expression)memberOfOpt };
            call.CallSignature.Position = signature.Position;
            return call;
        }

        public virtual LangElement Call(Span span, Name name, Span nameSpan, CallSignature signature, TypeRef typeRef)
        {
            var call = new DirectStMtdCall(span, new ClassConstUse(span, typeRef, new VariableNameRef(nameSpan, name.Value)), signature.Parameters, signature.GenericParams);
            call.CallSignature.Position = signature.Position;
            return call;
        }

        public virtual LangElement Call(Span span, TranslatedQualifiedName name, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(memberOfOpt == null || memberOfOpt is Expression);
            var call = new DirectFcnCall(span, name, signature.Parameters, signature.GenericParams) { IsMemberOf = (Expression)memberOfOpt };
            call.CallSignature.Position = signature.Position;
            return call;
        }
        public virtual ActualParam ActualParameter(Span span, LangElement expr, ActualParam.Flags flags)
        {
            Debug.Assert(expr != null && expr is Expression);
            return new ActualParam(span, (Expression)expr, flags);
        }

        public virtual LangElement ClassConstDecl(Span span, VariableName name, Span nameSpan, Span operatorSpan, LangElement initializer)
        {
            Debug.Assert(initializer == null || initializer is Expression);
            return new ClassConstantDecl(span, name.Value, nameSpan, (Expression)initializer) { AssignmentPosition = operatorSpan.StartOrInvalid };
        }

        public virtual LangElement ColonBlock(Span span, IEnumerable<LangElement> statements, Tokens endToken)
        {
            return new ColonBlockStmt(BlockSpan(span, statements), statements.CastToArray<Statement>(), endToken);
        }
        public virtual LangElement SimpleBlock(Span span, IEnumerable<LangElement> statements)
        {
            return new SimpleBlockStmt(BlockSpan(span, statements), statements.CastToArray<Statement>());
        }

        public virtual LangElement Concat(Span span, IEnumerable<LangElement> expressions, string label)
        {
            return new ConcatEx(span, expressions.CastToArray<Expression>()) { Label = label };
        }

        public virtual LangElement DeclList(Span span, PhpMemberAttributes attributes, int constPos, IEnumerable<LangElement> decls)
        {
            Debug.Assert(decls.All(e => e is FieldDecl) || decls.All(e => e is GlobalConstantDecl) || decls.All(e => e is ClassConstantDecl));
            if (decls.All(e => e is GlobalConstantDecl))
                return new GlobalConstDeclList(span, decls.CastToArray<GlobalConstantDecl>(), null) { ConstPosition = constPos };
            else if (decls.All(e => e is ClassConstantDecl))
                return new ConstDeclList(span, attributes, decls.CastToArray<ClassConstantDecl>(), null) { ConstPosition = constPos };
            else //if (decls.All(e => e is FieldDecl))
                return new FieldDeclList(span, attributes, decls.CastToArray<FieldDecl>(), null);
        }

        public virtual LangElement Do(Span span, Span whileSpan, LangElement body, LangElement cond, Span condSpan)
        {
            return new WhileStmt(span, WhileStmt.Type.Do, (Expression)cond, condSpan, (Statement)body)
            { WhilePosition = whileSpan.StartOrInvalid };
        }

        public virtual LangElement Echo(Span span, IEnumerable<LangElement> parameters)
        {
            return new EchoStmt(span, parameters.CastToArray<Expression>());
        }

        public virtual LangElement Unset(Span span, IEnumerable<LangElement> variables)
        {
            return new UnsetStmt(span, variables.CastToArray<VariableUse>());
        }

        public virtual LangElement Eval(Span span, LangElement code)
        {
            return new EvalEx(span, (Expression)code);
        }
        public virtual LangElement EncapsedExpression(Span span, LangElement expression, Tokens openDelimiter)
        {
            switch (openDelimiter)
            {
                case Tokens.T_LPAREN:
                    return new ParenthesisExpression(span, (Expression)expression);
                case Tokens.T_LBRACE:
                    return new BracesExpression(span, (Expression)expression);
                case Tokens.T_DOLLAR_OPEN_CURLY_BRACES:
                    return new DollarBracesExpression(span, (Expression)expression);
                default:
                    return expression;
            }
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
            return new IssetEx(span, variables.CastToArray<VariableUse>());
        }

        public virtual LangElement FieldDecl(Span span, VariableName name, Span assignSpan, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new FieldDecl(span, name.Value, (Expression)initializerOpt)
            { AssignOperatorPosition = assignSpan.StartOrInvalid };
        }

        public virtual LangElement For(Span span, IEnumerable<LangElement> init, IEnumerable<LangElement> cond, IEnumerable<LangElement> action, Span condSpan, LangElement body)
        {
            return new ForStmt(span, init.CastToArray<Expression>(), cond.CastToArray<Expression>(), action.CastToArray<Expression>(), condSpan, (Statement)body);
        }

        public virtual LangElement Foreach(Span span, LangElement enumeree, ForeachVar keyOpt, ForeachVar value, Span condSpan, LangElement body)
        {
            return new ForeachStmt(span, (Expression)enumeree, keyOpt, value, condSpan, (Statement)body);
        }

        public virtual LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType,
            Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, LangElement body, Span byRefPosition)
        {
            return new FunctionDecl(span, conditional, attributes, new NameRef(nameSpan, name.Value), aliasReturn,
                formalParams.AsArray(), formalParamsSpan, typeParamsOpt.AsArray(),
                (BlockStmt)body, null, returnType)
            { ReferencePosition = byRefPosition.StartOrInvalid };
        }

        public virtual LangElement Lambda(Span span, Span headingSpan, bool aliasReturn,
            TypeRef returnType, Span functionSpan, IEnumerable<FormalParam> formalParams,
            Span formalParamsSpan, IEnumerable<FormalParam> lexicalVars, Span useSpan,
            Span lexicalSpan, LangElement body, Span byRefPosition)
        {
            return new LambdaFunctionExpr(span, headingSpan,
                new Scope(), false, formalParams.AsArray(), formalParamsSpan, lexicalVars.AsArray(),
                (BlockStmt)body, returnType)
            {
                ReferencePosition = byRefPosition.StartOrInvalid,
                FunctionPosition = functionSpan.StartOrInvalid,
                UsePosition = useSpan.StartOrInvalid,
                UseSignaturePosition = lexicalSpan
            };
        }

        public virtual FormalParam Parameter(Span span, string name, Span nameSpan, TypeRef typeOpt, FormalParam.Flags flags, Span assignSpan, Expression initValue)
        {
            return new FormalParam(span, name, nameSpan, typeOpt, flags, initValue, null) { AssignmentPosition = assignSpan.StartOrInvalid };
        }

        public virtual LangElement GlobalCode(Span span, IEnumerable<LangElement> statements, NamingContext context)
        {
            _sourceUnit.Naming = context;
            var ast = new GlobalCode(span, statements.CastToArray<Statement>(), _sourceUnit);

            // link to parent nodes
            UpdateParentVisitor.UpdateParents(ast);

            //
            return ast;
        }

        public virtual LangElement GlobalConstDecl(Span span, bool conditional, VariableName name, Span nameSpan, Span operatorSpan, LangElement initializer)
        {
            return new GlobalConstantDecl(span, conditional, name.Value, nameSpan, (Expression)initializer) { AssignmentPosition = operatorSpan.StartOrInvalid };
        }

        public virtual LangElement Goto(Span span, string label, Span labelSpan)
        {
            return new GotoStmt(span, new VariableNameRef(labelSpan, label));
        }

        public virtual LangElement HaltCompiler(Span span, Span singatureSpan)
        {
            return new HaltCompiler(span) { SignaturePosition = singatureSpan };
        }

        public virtual LangElement If(Span span, LangElement cond, Span condSpan, LangElement body, LangElement elseOpt)
        {
            var conditions = new List<ConditionalStmt>() { new ConditionalStmt(span, (Expression)cond, condSpan, (Statement)body) };
            if (elseOpt != null)
            {
                Debug.Assert(elseOpt is IfStmt);
                conditions.AddRange(((IfStmt)elseOpt).Conditions);
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

        public virtual LangElement InstanceOf(Span span, int operatorPosition, LangElement expression, TypeRef typeRef)
        {
            return new InstanceOfEx(span, (Expression)expression, typeRef)
            { OperatorPosition = operatorPosition };
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

        public virtual LangElement List(Span span, IEnumerable<Item> targets, bool isOldNotation)
        {
            var items = targets.AsArray();
            return new ListEx(span, items.All(IsNull) ? null : items, isOldNotation);
        }

        public virtual LangElement Literal(Span span, object value)
        {
            if (value is long)
                return new LongIntLiteral(span, (long)value);
            else if (value is int)
                return new LongIntLiteral(span, (int)value);
            else if (value is double)
                return new DoubleLiteral(span, (double)value);
            else if (value is string)
            {
                var text = (string)value;
                if (text.Length >= 2 &&
                    (text[0] == '\'' && text[text.Length - 1] == '\'' ||
                    text[0] == '"' && text[text.Length - 1] == '"'))
                {
                    text = text.Substring(1, text.Length - 2);
                }
                return new StringLiteral(span, text);
            }
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

        public virtual LangElement Declare(Span span, IEnumerable<LangElement> decls, LangElement statementOpt)
        {
            Debug.Assert(statementOpt == null || statementOpt is Statement);
            return new DeclareStmt(span, (Statement)statementOpt);
        }

        public virtual LangElement Use(Span span, IEnumerable<UseBase> uses, AliasKind kind)
        {
            return new UseStatement(span, uses.AsArray(), kind);
        }

        public virtual LangElement New(Span span, TypeRef classNameRef, IEnumerable<ActualParam> argsOpt, Span argsPosition)
        {
            var call = new NewEx(span, classNameRef, argsOpt.AsArray());
            call.CallSignature.Position = argsPosition;
            return call;
        }

        public virtual LangElement NewArray(Span span, IEnumerable<Item> itemsOpt, bool isOldNotation)
        {
            var items = itemsOpt.AsArray();
            return new ArrayEx(span, items.All(IsNull) ? null : items, isOldNotation);
        }

        public virtual LangElement PHPDoc(Span span, LangElement block)
        {
            return new PHPDocStmt((PHPDocBlock)block);
        }

        public virtual LangElement Shell(Span span, LangElement command)
        {
            Debug.Assert(command is Expression);
            return new ShellEx(span, (Expression)command);
        }

        public virtual LangElement Switch(Span span, LangElement value, List<LangElement> block)
        {
            return new SwitchStmt(span, (Expression)value, block.CastToArray<SwitchItem>());
        }

        public virtual LangElement Case(Span span, LangElement valueOpt, LangElement block)
        {
            Debug.Assert(block is BlockStmt);
            if (valueOpt != null)
                return new CaseItem(span, (Expression)valueOpt, ((BlockStmt)block).Statements);
            else
                return new DefaultItem(span, ((BlockStmt)block).Statements);
        }

        public virtual LangElement TraitUse(Span span, IEnumerable<TypeRef> traits, LangElement adaptationsBlock)
        {
            Debug.Assert(traits != null);
            return new TraitsUse(span, traits.AsArray(), (TraitAdaptationBlock)adaptationsBlock);
        }

        public virtual LangElement TraitAdaptationPrecedence(Span span, Tuple<QualifiedNameRef, NameRef> name, IEnumerable<TypeRef> precedences)
        {
            Debug.Assert(precedences != null);
            return new TraitsUse.TraitAdaptationPrecedence(span, name, precedences.AsArray());
        }

        public virtual LangElement TraitAdaptationAlias(Span span, Tuple<QualifiedNameRef, NameRef> name, NameRef identifierOpt, PhpMemberAttributes? attributeOpt)
        {
            return new TraitsUse.TraitAdaptationAlias(span, name, identifierOpt, attributeOpt);
        }

        public virtual LangElement Global(Span span, List<LangElement> variables)
        {
            return new GlobalStmt(span, variables.CastToArray<SimpleVarUse>());
        }

        public virtual LangElement TryCatch(Span span, LangElement body, IEnumerable<LangElement> catches, LangElement finallyBlockOpt)
        {
            Debug.Assert(body is BlockStmt);
            return new TryStmt(span, (BlockStmt)body, catches.CastToArray<CatchItem>(), (FinallyItem)finallyBlockOpt);
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

        public virtual LangElement Type(Span span, Span keywordSpan, Span headingSpan, bool conditional, PhpMemberAttributes attributes, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, INamedTypeRef baseClassOpt, IEnumerable<INamedTypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            Debug.Assert(members != null && implements != null);
            return new NamedTypeDecl(span, headingSpan, conditional, attributes, false,
                new NameRef(nameSpan, name), typeParamsOpt.AsArray(),
                baseClassOpt, implements.AsArray(), members.CastToArray<TypeMemberDecl>(),
                bodySpan, null)
            { KeyworPosition = keywordSpan.StartOrInvalid };
        }

        public virtual TypeRef AnonymousTypeReference(Span span, Span keywordSpan, Span headingSpan, bool conditional, PhpMemberAttributes attributes, IEnumerable<FormalTypeParam> typeParamsOpt, INamedTypeRef baseClassOpt, IEnumerable<INamedTypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            Debug.Assert(members != null && implements != null);
            return new AnonymousTypeRef(span, new AnonymousTypeDecl(span, headingSpan,
                conditional, attributes, false, typeParamsOpt.AsArray(),
                baseClassOpt, implements.AsArray(), members.CastToArray<TypeMemberDecl>(),
                bodySpan, null)
            { KeyworPosition = keywordSpan.StartOrInvalid });
        }

        public virtual LangElement Method(Span span, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType, Span returnTypeSpan, string name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, IEnumerable<FormalParam> formalParams, Span formalParamsSpan, IEnumerable<ActualParam> baseCtorParams, LangElement body, Span functionPosition, Span modifierPosition, Span byRefPosition)
        {
            return new MethodDecl(span, new NameRef(nameSpan, name), aliasReturn, formalParams.AsArray(),
                formalParamsSpan, typeParamsOpt.AsArray(),
                (BlockStmt)body, attributes, baseCtorParams.AsArray(), null, returnType)
            {
                FunctionPosition = functionPosition.StartOrInvalid,
                ModifierPosition = modifierPosition.StartOrInvalid,
                ReferencePosition = byRefPosition.StartOrInvalid
            };
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
            return new IndirectVarUse(span, 1, (Expression)nameExpr) { IsMemberOf = (Expression)memberOfOpt };
        }

        public virtual LangElement Variable(Span span, string name, TypeRef typeRef)
        {
            Debug.Assert(typeRef != null);
            return new DirectStFldUse(span, typeRef, new VariableName(name), new Span(span.End - name.Length - 1, name.Length + 1));
        }

        public virtual LangElement Variable(Span span, string name, LangElement memberOfOpt, bool hasDollar)
        {
            int nameLength = name.Length + (hasDollar ? 1 : 0);
            return new DirectVarUse(new Span(span.End - nameLength, nameLength), name) { IsMemberOf = (Expression)memberOfOpt };
        }
        public virtual ForeachVar ForeachVariable(Span span, LangElement variable, bool alias)
        {
            if (variable is ListEx)
                return new ForeachVar((ListEx)variable);
            else
                return new ForeachVar((VariableUse)variable, alias);
        }

        public virtual TypeRef TypeReference(Span span, QualifiedName className)
        {
            //Debug.Assert(!className.IsPrimitiveTypeName);
            return new ClassTypeRef(span, className);
        }
        public virtual TypeRef AliasedTypeReference(Span span, QualifiedName className, TypeRef originalType)
        {
            Debug.Assert(!className.IsPrimitiveTypeName);
            return new TranslatedTypeRef(span, className, originalType);
        }
        public virtual TypeRef PrimitiveTypeReference(Span span, PrimitiveTypeRef.PrimitiveType tname)
        {
            return new PrimitiveTypeRef(span, tname);
        }
        public virtual TypeRef ReservedTypeReference(Span span, ReservedTypeRef.ReservedType typeName)
        {
            return new ReservedTypeRef(span, typeName);
        }
        public virtual TypeRef NullableTypeReference(Span span, LangElement className)
        {
            return new NullableTypeRef(span, (TypeRef)className);
        }
        public virtual TypeRef GenericTypeReference(Span span, LangElement className, List<TypeRef> genericParams)
        {
            return new GenericTypeRef(span, (TypeRef)className, genericParams);
        }
        public virtual TypeRef TypeReference(Span span, LangElement varName)
        {
            return new IndirectTypeRef(span, (Expression)varName);
        }
        public virtual TypeRef TypeReference(Span span, IEnumerable<LangElement> classes)
        {
            Debug.Assert(classes != null && classes.Count() > 0 && classes.All(c => c is TypeRef));

            if (classes.Count() == 1)
            {
                return (TypeRef)classes.First();
            }
            else
            {
                return new MultipleTypeRef(span, classes.CastToArray<TypeRef>());
            }
        }

        public virtual LangElement While(Span span, LangElement cond, Span condSpan, LangElement body)
        {
            Debug.Assert(cond is Expression && body is Statement);
            return new WhileStmt(span, WhileStmt.Type.While, (Expression)cond, condSpan, (Statement)body)
            { WhilePosition = span.Start };
        }

        public virtual LangElement Yield(Span span, LangElement keyOpt, Span operatorSpan, LangElement valueOpt)
        {
            return new YieldEx(span, (Expression)keyOpt, (Expression)valueOpt) { OperatorPosition = operatorSpan.StartOrInvalid };
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
            return new StaticStmt(span, staticVariables.CastToArray<StaticVarDecl>());
        }
        public virtual LangElement StaticVarDecl(Span span, VariableName name, Span operatorSpan, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new StaticVarDecl(span, name, (Expression)initializerOpt) { AssignmentPosition = operatorSpan.StartOrInvalid };
        }

        public virtual LangElement ConstUse(Span span, TranslatedQualifiedName name)
        {
            return new GlobalConstUse(span, name);
        }

        public virtual LangElement ClassConstUse(Span span, TypeRef tref, Name name, Span nameSpan)
        {
            return new ClassConstUse(span, tref, new VariableNameRef(nameSpan, name.Value));
        }

        public virtual LangElement ConditionalEx(Span span, LangElement condExpr, Span questionSpan, LangElement trueExpr, Span colonSpan, LangElement falseExpr)
        {
            return new ConditionalEx(span, (Expression)condExpr, (Expression)trueExpr, (Expression)falseExpr)
            { QuestionPosition = questionSpan.Start, ColonPosition = colonSpan.Start };
        }
    }
}
