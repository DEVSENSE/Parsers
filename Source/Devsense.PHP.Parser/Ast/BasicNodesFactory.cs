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
using System.Xml.Linq;
using Devsense.PHP.Ast.DocBlock;
using Devsense.PHP.Errors;
using Devsense.PHP.Text;
using Devsense.PHP.Utilities;
using static Devsense.PHP.Syntax.Ast.EncapsedExpression;

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
        public SourceUnit SourceUnit { get; }

        /// <summary>
        /// Optional reference to error sink to handle AST validity errors.
        /// </summary>
        protected virtual IErrorSink<Span> ErrorSink => null;

        static bool IsAllNull(ArrayItem[] arr)
        {
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].IsDefault == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public BasicNodesFactory(SourceUnit sourceUnit)
        {
            this.SourceUnit = sourceUnit ?? throw new ArgumentNullException(nameof(sourceUnit));
        }

        public virtual LangElement ArrayItem(Span span, bool braces, LangElement expression, LangElement indexOpt)
        {
            return ItemUse.Create(span, (Expression)expression, (Expression)indexOpt, isBraces: braces);
        }
        public virtual ArrayItem ArrayItemValue(Span span, LangElement indexOpt, LangElement valueExpr)
        {
            return Ast.ArrayItem.CreateValueItem((Expression)indexOpt, (Expression)valueExpr);
        }
        public virtual ArrayItem ArrayItemRef(Span span, LangElement indexOpt, LangElement variable)
        {
            return Ast.ArrayItem.CreateByRefItem((Expression)indexOpt, (VariableUse)variable);
        }

        public virtual ArrayItem ArrayItemSpread(Span span, LangElement expression)
        {
            return Ast.ArrayItem.CreateSpreadItem((Expression)expression);
        }

        public virtual LangElement Assignment(Span span, LangElement target, LangElement value, Operations assignOp)
        {
            if (assignOp == Operations.AssignRef)
            {
                return new RefAssignEx(span, (VarLikeConstructUse)target, (Expression)value);
            }
            else
            {
                return new ValueAssignEx(span, assignOp, (VarLikeConstructUse)target, (Expression)value);
            }
        }

        //public virtual LangElement BinaryOperation(Span span, Operations operation, LangElement leftExpression, LangElement rightExpression)
        //{
        //    return BinaryEx.Create(span, operation, (Expression)leftExpression, (Expression)rightExpression);
        //}

        public virtual LangElement BinaryOperation(Span span, Tokens operation, LangElement leftExpression, LangElement rightExpression)
        {
            return BinaryEx.Create(span, operation, (Expression)leftExpression, (Expression)rightExpression);
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
            return new IndirectStMtdCall(span, typeRef, (Expression)nameExpr, signature);
        }

        public virtual LangElement Call(Span span, LangElement nameExpr, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(nameExpr is Expression);
            return new IndirectFcnCall(span, (Expression)nameExpr, signature, (Expression)memberOfOpt);
        }

        public virtual LangElement Call(Span span, Name name, Span nameSpan, CallSignature signature, TypeRef typeRef)
        {
            return new DirectStMtdCall(span, typeRef, new NameRef(nameSpan, name), signature);
        }

        public virtual LangElement Call(Span span, TranslatedQualifiedName name, CallSignature signature, LangElement memberOfOpt)
        {
            Debug.Assert(memberOfOpt == null || memberOfOpt is Expression);
            return DirectFcnCall.Create(span, name, signature, isMemberOf: (Expression)memberOfOpt);
        }

        public virtual LangElement Clone(Span span, CallSignature signature)
        {
            if (signature.Span.IsValid || signature.Parameters.Length != 1 || signature.IsCallableConvert)
            {
                // clone as a function
                return DirectFcnCall.CreateClone(span, signature);
            }
            else
            {
                // clone expr
                // for backward compatibility
                return UnaryOperation(span, Operations.Clone, signature.Parameters[0].Expression);
            }
        }

        public virtual ActualParam ActualParameter(Span span, LangElement expr, ActualParam.Flags flags, VariableNameRef? nameOpt = default)
        {
            Debug.Assert(expr is Expression);

            if (nameOpt.HasValue)
            {
                Debug.Assert(!string.IsNullOrWhiteSpace(nameOpt.Value.Name.Value));
                Debug.Assert(nameOpt.Value.Name.Value != ":");
            }

            return new ActualParam(span, (Expression)expr, flags, nameOpt);
        }

        public virtual LangElement ClassConstDecl(Span span, VariableName name, Span nameSpan, LangElement initializer)
        {
            Debug.Assert(initializer == null || initializer is Expression);
            return new ClassConstantDecl(span, name.Value, nameSpan, (Expression)initializer);
        }

        public virtual LangElement EnumCase(Span span, string name, Span nameSpan, LangElement expression)
        {
            return new EnumCaseDecl(span, name, nameSpan, (Expression)expression);
        }

        public virtual LangElement ColonBlock(Span span, IEnumerable<LangElement> statements, Tokens endToken)
        {
            return new ColonBlockStmt(BlockSpan(span, statements), statements.CastToArray<Statement>(), endToken);
        }
        public virtual LangElement SimpleBlock(Span span, IEnumerable<LangElement> statements)
        {
            return new SimpleBlockStmt(BlockSpan(span, statements), statements.CastToArray<Statement>());
        }
        public virtual LangElement Concat(Span span, IEnumerable<LangElement> expressions)
        {
            return new ConcatEx(span, expressions.CastToArray<Expression>());
        }

        public virtual LangElement DeclList(Span span, PhpMemberAttributes attributes, IReadOnlyList<LangElement>/*!!*/decls, TypeRef type)
        {
            Debug.Assert(decls.Count != 0);
            Debug.Assert(decls.All(e => e is FieldDecl) || decls.All(e => e is GlobalConstantDecl) || decls.All(e => e is ClassConstantDecl));

            if (decls[0] is FieldDecl)
            {
                Debug.Assert(decls.All(e => e is FieldDecl));
                Debug.Assert(typeof(IReadOnlyList<FieldDecl>).IsAssignableFrom(typeof(FieldDecl)));

                return new FieldDeclList(span, attributes,
                    fields: decls.Count == 1
                        ? (IReadOnlyList<FieldDecl>)decls[0] // HACK // FieldDecl is IReadOnlyList<FieldDecl> with itself
                        : decls.CastToArray<FieldDecl>()
                        ,
                    type: type
                );
            }
            else if (decls[0] is ClassConstantDecl)
            {
                Debug.Assert(decls.All(e => e is ClassConstantDecl));
                return new ConstDeclList(span, attributes, decls.CastToArray<ClassConstantDecl>(), type);
            }
            else if (decls[0] is GlobalConstantDecl)
            {
                Debug.Assert(decls.All(e => e is GlobalConstantDecl));
                return new GlobalConstDeclList(span, decls.CastToArray<GlobalConstantDecl>());
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public virtual LangElement Do(Span span, LangElement body, LangElement cond, Span parenthesesSpan, Span whileSpan)
        {
            return new WhileStmt(span, WhileStmt.Type.Do, (Expression)cond, parenthesesSpan, whileSpan, (Statement)body);
        }

        public virtual LangElement Echo(Span span, IReadOnlyList<LangElement> parameters)
        {
            return parameters.Count == 1
                ? EchoStmt.CreateEcho(span, (IExpression)parameters[0])
                : EchoStmt.CreateEcho(span, parameters.CastToArray<IExpression>())
                ;
        }

        public virtual LangElement Unset(Span span, IEnumerable<LangElement> variables)
        {
            var arr = variables.CastToArray<IExpression>();
            CheckVarUse(arr);
            return new UnsetStmt(span, arr);
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
                    throw new ArgumentOutOfRangeException(nameof(openDelimiter), openDelimiter, string.Empty);
            }
        }

        public virtual LangElement StringEncapsedExpression(Span span, LangElement expression, Tokens openDelimiter)
        {
            switch (openDelimiter)
            {
                case Tokens.T_SINGLE_QUOTES:
                    return new SingleQuotedExpression(span, (Expression)expression);
                case Tokens.T_DOUBLE_QUOTES:
                    return new DoubleQuotedExpression(span, (Expression)expression);
                case Tokens.T_BACKQUOTE:
                    return new BackQuotedExpression(span, (Expression)expression);
                default:
                    throw new ArgumentOutOfRangeException(nameof(openDelimiter), openDelimiter, string.Empty);
            }
        }

        public virtual LangElement HeredocExpression(Span span, LangElement expression, Tokens quoteStyle, Lexer.HereDocTokenValue heredoc)
        {
            Debug.Assert(heredoc != null);

            switch (quoteStyle)
            {
                case Tokens.T_SINGLE_QUOTES:
                    return new NowDocExpression(span, (Expression)expression, heredoc.Label);
                default:
                    return new HereDocExpression(span, (Expression)expression, heredoc.Label, quoteStyle == Tokens.T_DOUBLE_QUOTES);
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
        public virtual LangElement EmptyStmt(Span span) => span.Length == 1 ? new EmptyStmt(span) : null;

        public virtual LangElement Isset(Span span, IEnumerable<LangElement> variables)
        {
            var arr = variables.CastToArray<IExpression>();

            CheckVarUse(arr);

            return new IssetEx(span, arr);
        }

        void CheckVarUse(IExpression[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                CheckVarUse(arr[i]);
            }
        }

        protected virtual void CheckVarUse(IExpression expr)
        {
            if (expr != null)
            {
                if (expr is EncapsedExpression encapsed)
                {
                    expr = encapsed.Expression;
                }

                if (expr is not VariableUse)
                {
                    ErrorSink?.Error(expr.Span, FatalErrors.CheckVarUseFault);
                }
            }
        }

        public virtual LangElement FieldDecl(Span span, VariableName name, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new FieldDecl(span, name.Value, (Expression)initializerOpt);
        }

        public virtual LangElement For(Span span, IEnumerable<LangElement> init, IEnumerable<LangElement> cond, IEnumerable<LangElement> action, Span parenthesesSpan, LangElement body)
        {
            return new ForStmt(span, init.CastToArray<Expression>(), cond.CastToArray<Expression>(), action.CastToArray<Expression>(), parenthesesSpan, (Statement)body);
        }

        public virtual LangElement Foreach(Span span, LangElement enumeree, ForeachVar keyOpt, ForeachVar value, LangElement body)
        {
            return new ForeachStmt(span, (Expression)enumeree, keyOpt, value, (Statement)body);
        }

        public virtual LangElement Function(Span span, bool conditional, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType,
            Name name, Span nameSpan, FormalTypeParam[] typeParamsOpt, FormalParam[] formalParams, Span formalParamsSpan, LangElement body)
        {
            return new FunctionDecl(span, conditional, attributes, new NameRef(nameSpan, name.Value), aliasReturn,
                formalParams ?? EmptyArray<FormalParam>.Instance, formalParamsSpan, typeParamsOpt,
                (BlockStmt)body, returnType);
        }

        public virtual LangElement Lambda(Span span, Span headingSpan,
            bool aliasReturn, TypeRef returnType, FormalParam[] formalParams,
            Span formalParamsSpan, FormalParam[] lexicalVars, LangElement body)
        {
            return new LambdaFunctionExpr(
                span, headingSpan,
                new Signature(aliasReturn, formalParams ?? EmptyArray<FormalParam>.Instance, formalParamsSpan),
                lexicalVars ?? EmptyArray<FormalParam>.Instance, (BlockStmt)body, returnType);
        }

        public virtual LangElement ArrowFunc(Span span, Span headingSpan,
            bool aliasReturn, TypeRef returnType, FormalParam[] formalParams,
            Span formalParamsSpan, LangElement expression)
        {
            return new ArrowFunctionExpr(
                span, headingSpan,
                new Signature(aliasReturn, formalParams ?? EmptyArray<FormalParam>.Instance, formalParamsSpan),
                (Expression)expression, returnType);
        }

        public virtual LangElement PropertyDecl(Span span, PhpMemberAttributes modifiers, TypeRef propertyType, VariableNameRef name, IList<LangElement> hooks, IExpression initialValue)
        {
            return new PropertyDecl(span, name, modifiers, hooks.CastToArray<PropertyHookDecl>(), propertyType, initialValue);
        }

        public virtual LangElement PropertyHook(
            Span span,
            bool aliasReturn, PhpMemberAttributes attributes,
            Name name, Span nameSpan,
            FormalParam[] formalParams, Span formalParamsSpan,
            LangElement body
        )
        {
            return new PropertyHookDecl(
                span,
                attributes,
                new NameRef(nameSpan, name),
                new Signature(aliasReturn, formalParams, formalParams != null ? formalParamsSpan : Span.Invalid),
                body
            );
        }

        public virtual FormalParam Parameter(Span span, VariableNameRef name, TypeRef typeOpt, FormalParam.Flags flags, Expression initValue,
            PhpMemberAttributes visibility,
            List<LangElement> property_hooks
        )
        {
            return FormalParam.Create(span, name, typeOpt, flags, initValue,
                constructorPropertyVisibility: visibility,
                property_hooks: property_hooks != null ? property_hooks.CastToArray<PropertyHookDecl>() : null
            );
        }

        public virtual LangElement GlobalCode(Span span, Statement[] statements, NamingContext context)
        {
            SourceUnit.Naming = context;
            var ast = new GlobalCode(span, statements ?? EmptyArray<Statement>.Instance, SourceUnit);

            // link to parent nodes
            UpdateParentHelper.UpdateParents(ast);

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

        public virtual LangElement If(ConditionalStmt conditions)
        {
            // collect whole "if" span
            var span = conditions.Span;

            foreach (var item in conditions)
            {
                span = Span.Combine(span, item.Span);
            }

            // create element
            return new IfStmt(span, conditions ?? throw new ArgumentNullException(nameof(conditions)));
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
            return EchoStmt.CreateHtml(span, html);
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
            return new LabelStmt(span, label, labelSpan);
        }

        public virtual LangElement LineComment(Span span, string content)
        {
            throw new NotImplementedException();
        }

        public virtual LangElement List(Span span, ArrayItem[] items, bool isOldNotation)
        {
            return ArrayEx.CreateList(span, IsAllNull(items) ? null : items, !isOldNotation);
        }

        public virtual LangElement Literal(Span span, object value, ReadOnlySpan<char> originalValue)
        {
            var result = Ast.Literal.Create(span, value, originalValue);
            result.SourceText = originalValue.IsEmpty ? null : originalValue.ToString();
            return result;
        }

        public virtual LangElement Literal(Span span, long value, ReadOnlySpan<char> originalValue)
        {
            var result = Ast.Literal.Create(span, value, originalValue);
            result.SourceText = originalValue.IsEmpty ? null : originalValue.ToString();
            return result;
        }

        public virtual LangElement Literal(Span span, double value, ReadOnlySpan<char> originalValue)
        {
            var result = Ast.Literal.Create(span, value, originalValue);
            result.SourceText = originalValue.IsEmpty ? null : originalValue.ToString();
            return result;
        }

        public virtual LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, NamingContext context)
        {
            return new NamespaceDecl(span, name.HasValue ? new QualifiedNameRef(nameSpan, name.Value) : QualifiedNameRef.Invalid, true)
            {
                Naming = context
            };
        }

        public virtual LangElement Namespace(Span span, QualifiedName? name, Span nameSpan, LangElement block, NamingContext context)
        {
            Debug.Assert(block != null);
            return new NamespaceDecl(span, name.HasValue ? new QualifiedNameRef(nameSpan, name.Value) : QualifiedNameRef.Invalid, false)
            {
                Naming = context,
                Body = (BlockStmt)block
            };
        }

        public virtual LangElement Declare(Span span, IEnumerable<LangElement> decls, LangElement statement)
        {
            Debug.Assert(statement == null || statement is Statement);
            return new DeclareStmt(span, decls.CastToArray<GlobalConstantDecl>(), (Statement)statement);
        }

        public virtual LangElement Use(Span span, UseBase[] uses, AliasKind kind)
        {
            return new UseStatement(span, uses, kind);
        }

        public virtual LangElement New(Span span, TypeRef classNameRef, ActualParam[] argsOpt, Span argsPosition)
        {
            return new NewEx(span, classNameRef, argsOpt ?? ActualParam.EmptyArray, argsPosition);
        }

        public virtual LangElement NewArray(Span span, IEnumerable<ArrayItem> itemsOpt, bool isOldNotation)
        {
            var items = itemsOpt.AsArray();
            return ArrayEx.CreateArray(span, IsAllNull(items) ? null : items, !isOldNotation);
        }

        public virtual IDocBlock CreateDocBlock(Span span, string source) => DefaultDocBlockFactory.Instance.CreateDocBlock(span, source);

        public virtual LangElement PHPDoc(Span span, IDocBlock block)
        {
            return new PHPDocStmt(block);
        }

        public virtual LangElement Shell(Span span, LangElement command)
        {
            Debug.Assert(command is Expression);
            return new ShellEx(span, (Expression)command);
        }

        public virtual LangElement Switch(Span span, LangElement value, Span valueSpan, List<LangElement> block, Tokens endToken, Span endTokenSpan)
        {
            return new SwitchStmt(
                span,
                (Expression)value,
                valueSpan,
                block.CastToArray<SwitchItem>(),
                endToken,
                endTokenSpan
            );
        }

        public virtual LangElement Case(Span span, LangElement valueOpt, LangElement block)
        {
            Debug.Assert(block is BlockStmt);
            span = span.IsValid && block.Span.IsValid && span.Start <= block.Span.End ? Span.FromBounds(span.Start, block.Span.End) : Span.Invalid;
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

        public virtual LangElement TraitAdaptationPrecedence(Span span, Tuple<TypeRef, NameRef> name, IEnumerable<TypeRef> precedences)
        {
            Debug.Assert(precedences != null);
            return new TraitsUse.TraitAdaptationPrecedence(span, name, precedences.AsArray());
        }

        public virtual LangElement TraitAdaptationAlias(Span span, Tuple<TypeRef, NameRef> name, NameRef identifierOpt, PhpMemberAttributes? attributeOpt)
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
            return new ThrowEx(span, (Expression)expression);
        }

        public virtual LangElement Type(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, Name name, Span nameSpan, IEnumerable<FormalTypeParam> typeParamsOpt, INamedTypeRef baseClassOpt, INamedTypeRef[] implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            Debug.Assert(members != null && implements != null);
            return new NamedTypeDecl(span, headingSpan, conditional, attributes, false,
                new NameRef(nameSpan, name), typeParamsOpt.AsArray(),
                baseClassOpt, implements, members.CastToArray<TypeMemberDecl>(),
                bodySpan);
        }

        public virtual TypeRef AnonymousTypeReference(Span span, Span headingSpan, bool conditional, PhpMemberAttributes attributes, IEnumerable<FormalTypeParam> typeParamsOpt, INamedTypeRef baseClassOpt, IEnumerable<INamedTypeRef> implements, IEnumerable<LangElement> members, Span bodySpan)
        {
            Debug.Assert(members != null && implements != null);
            return new AnonymousTypeRef(span, new AnonymousTypeDecl(span, headingSpan,
                conditional, attributes, false, typeParamsOpt.AsArray(),
                baseClassOpt, implements.AsArray(), members.CastToArray<TypeMemberDecl>(),
                bodySpan));
        }

        public virtual LangElement Method(Span span, bool aliasReturn, PhpMemberAttributes attributes, TypeRef returnType,
            Span returnTypeSpan, string name, Span nameSpan, FormalTypeParam[] typeParamsOpt,
            FormalParam[] formalParams, Span formalParamsSpan, ActualParam[] baseCtorParams, LangElement body)
        {
            return new MethodDecl(span, new NameRef(nameSpan, name), aliasReturn,
                formalParams.AsArray(), formalParamsSpan,
                typeParamsOpt.AsArray(),
                (BlockStmt)body, attributes,
                baseCtorParams.AsArray(),
                returnType
            );
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
            return new IndirectVarUse(span, 1, (Expression)nameExpr, isMemberOf: (Expression)memberOfOpt);
        }

        public virtual LangElement Variable(Span span, string name, TypeRef typeRef)
        {
            Debug.Assert(typeRef != null);
            return new DirectStFldUse(span, typeRef, new VariableNameRef(span.End - name.Length - 1, new VariableName(name)));
        }

        public virtual LangElement Variable(Span span, string name, LangElement memberOfOpt, bool hasDollar)
        {
            int nameLength = name.Length + (hasDollar ? 1 : 0);

            return DirectVarUse.Create(
                new Span(span.End - nameLength, nameLength),
                new VariableName(name),
                (Expression)memberOfOpt
            );
        }

        public virtual ForeachVar ForeachVariable(Span span, LangElement variable, bool alias)
        {
            if (variable is IArrayExpression arrexpr)
                return new ForeachVar(arrexpr);
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
            return varName as TypeRef ?? new IndirectTypeRef(span, (Expression)varName);
        }
        public virtual TypeRef TypeReference(Span span, IEnumerable<LangElement> classes)
        {
            Debug.Assert(classes != null && classes.Count() > 0 && classes.All(c => c is TypeRef));

            var typearr = classes.CastToArray<TypeRef>();

            // (PHP 8) false|null
            bool IsPHP8PrimitiveTypeName(INamedTypeRef named, out PrimitiveTypeRef.PrimitiveType ptype)
            {
                if (named is TranslatedTypeRef tt)
                {
                    named = tt.OriginalType as INamedTypeRef;
                }

                if (named != null)
                {
                    var qname = named.ClassName;

                    if (qname.IsSimpleName && !qname.IsFullyQualifiedName)
                    {
                        if (qname.Equals(QualifiedName.Null))
                        {
                            ptype = PrimitiveTypeRef.PrimitiveType.@null;
                            return true;
                        }

                        if (qname.Equals(QualifiedName.False))
                        {
                            ptype = PrimitiveTypeRef.PrimitiveType.@false;
                            return true;
                        }
                    }
                }

                //
                ptype = default(PrimitiveTypeRef.PrimitiveType);
                return false;
            }

            for (int i = typearr.Length - 1; i >= 0; i--)
            {
                // null|true|false -> primitive type reference
                if (typearr[i] is INamedTypeRef named &&
                    IsPHP8PrimitiveTypeName(named, out var ptype))
                {
                    typearr[i] = new PrimitiveTypeRef(named.Span, ptype);
                }
            }

            //
            return typearr.Length == 1
                ? typearr[0]
                : new MultipleTypeRef(span, typearr)
                ;
        }

        public virtual TypeRef IntersectionTypeReference(Span span, IEnumerable<LangElement> classes)
        {
            Debug.Assert(classes != null && classes.Count() > 0 && classes.All(c => c is TypeRef));

            var typearr = classes is IList<TypeRef> list ? list.ToArray() : classes.CastToArray<TypeRef>();

            if (typearr.Length == 1)
            {
                return typearr[0];
            }

            return new IntersectionTypeRef(span, typearr);
        }

        public virtual LangElement While(Span span, LangElement cond, Span parenthesesSpan, LangElement body)
        {
            Debug.Assert(cond is Expression && body is Statement);
            return new WhileStmt(
                span,
                WhileStmt.Type.While,
                (Expression)cond,
                parenthesesSpan,
                span.IsValid ? new Span(span.Start, "while".Length) : Span.Invalid,
                (Statement)body);
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
            return new StaticStmt(span, staticVariables.CastToArray<StaticVarDecl>());
        }
        public virtual LangElement StaticVarDecl(Span span, VariableName name, LangElement initializerOpt)
        {
            Debug.Assert(initializerOpt == null || initializerOpt is Expression);
            return new StaticVarDecl(span, name, (Expression)initializerOpt);
        }

        public virtual LangElement ConstUse(Span span, TranslatedQualifiedName name)
        {
            return new GlobalConstUse(span, name);
        }

        public virtual LangElement ClassConstUse(Span span, TypeRef tref, Name name, Span nameSpan)
        {
            return new DirectClassConstUse(span, tref, nameSpan, name.Value);
        }

        public virtual LangElement ClassConstUse(Span span, TypeRef tref, LangElement expr)
        {
            return new IndirectClassConstUse(span, tref, (IExpression)expr);
        }

        public virtual LangElement ConditionalEx(Span span, LangElement condExpr, LangElement trueExpr, LangElement falseExpr)
        {
            return new ConditionalEx(span, (Expression)condExpr, (Expression)trueExpr, (Expression)falseExpr);
        }

        public virtual LangElement Attribute(Span span, TypeRef classref, CallSignature signature)
        {
            return new AttributeElement(span, classref, signature);
        }

        public virtual LangElement AttributeGroup(Span span, IAttributeElement[] attributes)
        {
            return new AttributeGroup(
                span, attributes ?? throw new ArgumentNullException(nameof(attributes))
            );
        }

        public virtual IMatchEx Match(Span span, LangElement value, List<LangElement> arms)
        {
            return new MatchEx(span, (Expression)value, arms.CastToArray<MatchArm>());
        }

        public virtual IMatchArm MatchArm(Span span, IExpression[] conditions, LangElement expression)
        {
            return new MatchArm(span, conditions, (Expression)expression);
        }
    }
}
