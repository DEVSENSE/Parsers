using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax.Ast;
using System.Diagnostics;
using System.Globalization;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Options specifying how <see cref="TokenVisitor"/> synthesizes tokens from the syntax tree if not specified.
    /// </summary>
    public interface ITokenComposer
    {
        /// <summary>
        /// Consumes a literal.
        /// Calls corresponding <see cref="ConsumeToken"/>.
        /// </summary>
        void ConsumeLiteral(Literal literal);

        /// <summary>
        /// Consume modifier tokens.
        /// Calls corresponding <see cref="ConsumeToken"/>.
        /// </summary>
        /// <param name="element">Original declaration element.</param>
        /// <param name="modifiers">Modifiers.</param>
        /// <param name="span">Optional. Modifiers span.</param>
        void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span = default(Span));

        /// <summary>
        /// Consumes next token.
        /// </summary>
        /// <param name="token">The token ID.</param>
        /// <param name="text">Token source code content - synthesized or passed from original source code.</param>
        /// <param name="position">Original token position in source code.</param>
        void ConsumeToken(Tokens token, string text, Span position);
    }

    #region DefaultTokenVisitorOptions

    /// <summary>
    /// A default options implementation with default values.
    /// </summary>
    public class DefaultTokenComposer : ITokenComposer
    {
        public static readonly ITokenComposer Instance = new DefaultTokenComposer();

        protected DefaultTokenComposer() { }

        /// <summary>
        /// Shortcut for <see cref="ConsumeToken(Tokens, string, Span)"/>.
        /// </summary>
        protected void ConsumeToken(Tokens token, Span position) => ConsumeToken(token, TokenFacts.GetTokenText(token), position);

        /// <inheritdoc />
        public virtual void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            if ((modifiers & PhpMemberAttributes.Private) != 0) ConsumeToken(Tokens.T_PRIVATE, span);
            if ((modifiers & PhpMemberAttributes.Protected) != 0) ConsumeToken(Tokens.T_PROTECTED, span);

            if ((modifiers & PhpMemberAttributes.Static) != 0) ConsumeToken(Tokens.T_STATIC, span);
            if ((modifiers & PhpMemberAttributes.Abstract) != 0) ConsumeToken(Tokens.T_ABSTRACT, span);
            if ((modifiers & PhpMemberAttributes.Final) != 0) ConsumeToken(Tokens.T_FINAL, span);
        }

        /// <inheritdoc />
        public virtual void ConsumeToken(Tokens token, string text, Span position)
        {
            // to be overwritten in derived class
            Debug.WriteLine("ConsumeToken {0}: {1}", token.ToString(), text);
        }

        /// <inheritdoc />
        public virtual bool IsOldArraySyntax(ArrayEx node) => false;

        public virtual void ConsumeLiteral(Literal literal)
        {
            if (literal is BoolLiteral)
            {
                var value = ((BoolLiteral)literal).Value.ToString();
                ConsumeToken(Tokens.T_STRING, /*IsUpperCase(x) ? value.ToUpperInvariant() :*/ value.ToLowerInvariant(), literal.Span);
            }
            else if (literal is DoubleLiteral)
            {
                ConsumeToken(Tokens.T_DNUMBER, ((DoubleLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span);
            }
            else if (literal is NullLiteral)
            {
                ConsumeToken(Tokens.T_STRING, /*_composer.IsUpperCase(x) ? "NULL" :*/ "null", literal.Span);
            }
            else if (literal is LongIntLiteral)
            {
                ConsumeToken(Tokens.T_LNUMBER, ((LongIntLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    #endregion

    public class TokenVisitor : TreeContextVisitor
    {
        readonly ITokenComposer _composer;
        readonly ISourceTokenProvider _provider;

        private void ProcessToken(Tokens target, Span span)
        {
            var token = _provider.GetTokenAt(span, target, new SourceToken(target, Span.Invalid));
            ConsumeToken(token.Token, token.Span);
        }

        private void ProcessToken(Tokens target, string text, Span span)
        {
            var token = _provider.GetTokenAt(span, target, new SourceToken(target, Span.Invalid));
            ConsumeToken(token.Token, text, token.Span);
        }

        public TokenVisitor(TreeContext initialContext, ITokenComposer composer, ISourceTokenProvider tokenProvider) : base(initialContext)
        {
            _composer = composer ?? DefaultTokenComposer.Instance;  // TODO: to TreeContext
            _provider = tokenProvider ?? SourceTokenProviderFactory.CreateEmptyProvider();
        }

        /// <summary>
        /// Invoked when a token is visited.
        /// </summary>
        /// <param name="token">Token id.</param>
        /// <param name="text">Textual representation of <paramref name="token"/>.</param>
        /// <param name="position">Optional. Original position in source code.</param>
        public void ConsumeToken(Tokens token, string text, Span position)
        {
            _composer.ConsumeToken(token, text, position);
        }

        protected void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            _composer.ConsumeModifiers(element, modifiers, span);
        }

        /// <summary>
        /// Shortcut for <see cref="ConsumeToken(Tokens, string, Span)"/>.
        /// </summary>
        protected void ConsumeToken(Tokens token, Span position) => ConsumeToken(token, TokenFacts.GetTokenText(token), position);

        #region Single Nodes Overrides

        public override void VisitElement(LangElement element)
        {
            base.VisitElement(element);
        }

        public override void VisitActualParam(ActualParam x)
        {
            if (x.IsUnpack)
            {
                ProcessToken(Tokens.T_ELLIPSIS, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Expression.Span));
            }
            if (x.Ampersand)
            {
                ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Expression.Span));
            }
            VisitElement(x.Expression);
        }

        public override void VisitAnonymousTypeDecl(AnonymousTypeDecl x)
        {
            VisitTypeDecl(x);
        }

        public override void VisitAnonymousTypeRef(AnonymousTypeRef x)
        {
            VisitElement(x.TypeDeclaration);
        }

        public override void VisitArrayEx(ArrayEx x)
        {
            var itemSpan = x.Items.ItemsSpan();
            if (x.IsOldNotation)
            {
                ProcessToken(Tokens.T_ARRAY, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span);
                ProcessToken(Tokens.T_LPAREN, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span);
                VisitElementList(x.Items, t => VisitArrayItem((Item)t), Tokens.T_COMMA, ",");
                ProcessToken(Tokens.T_RPAREN, itemSpan.IsValid ? SpanUtils.SpanIntermission(itemSpan, x.Span.End) : x.Span);
            }
            else
            {
                ProcessToken(Tokens.T_LBRACKET, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span);
                VisitElementList(x.Items, t => VisitArrayItem((Item)t), Tokens.T_COMMA, ",");
                ProcessToken(Tokens.T_RBRACKET, itemSpan.IsValid ? SpanUtils.SpanIntermission(itemSpan, x.Span.End) : x.Span);
            }
        }

        public override void VisitArrayItem(Item item)
        {
            if (item != null)
            {
                var valueSpan = item is ValueItem ? ((ValueItem)item).ValueExpr.Span : ((RefItem)item).RefToGet.Span;
                if (item.Index != null)
                {
                    VisitElement(item.Index);
                    ProcessToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SpanIntermission(item.Index.Span, valueSpan));
                }

                if (item is ValueItem)
                {
                    VisitElement(((ValueItem)item).ValueExpr);
                }
                else if (item is RefItem)
                {
                    // TODO - fix items
                    var tokens = _provider.GetTokens(SpanUtils.SafeSpan(0, valueSpan.StartOrInvalid));
                    var amp = tokens.LastOrDefault(t => t.Token == Tokens.T_AMP) ?? new SourceToken(Tokens.T_AMP, Span.Invalid);
                    ConsumeToken(amp.Token, amp.Span);
                    VisitElement(((RefItem)item).RefToGet);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public override void VisitAssertEx(AssertEx x)
        {
            ProcessToken(Tokens.T_STRING, "assert", SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.CodeEx.Span));
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.CodeEx.Span));
            VisitElement(x.CodeEx);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.CodeEx.Span, x.Span.End));
        }

        public sealed override void VisitAssignEx(AssignEx x) { throw new InvalidOperationException(); }

        public override void VisitBinaryEx(BinaryEx x)
        {
            VisitElement(x.LeftExpr);
            ProcessToken(TokenFacts.GetOperationToken(x.Operation), x.OperatorSpan);
            VisitElement(x.RightExpr);
        }

        public override void VisitBinaryStringLiteral(BinaryStringLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitBlockStmt(BlockStmt x)
        {
            if (x is ColonBlockStmt)
            {
                ColonBlockStmt block = (ColonBlockStmt)x;
                var openingText = TokenFacts.GetTokenText(block.OpeningToken);
                ConsumeToken(block.OpeningToken, openingText, new Span(x.Span.StartOrInvalid, openingText.Length));
                base.VisitBlockStmt(x);
                var closingText = TokenFacts.GetTokenText(block.ClosingToken);
                ConsumeToken(block.ClosingToken, closingText, SpanUtils.SafeSpan(x.Span.End, closingText.Length));
                if (block.ClosingToken != Tokens.T_ELSE && block.ClosingToken != Tokens.T_ELSEIF)
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End + closingText.Length, 1));
                }
            }
            else
            {
                ConsumeToken(Tokens.T_LBRACE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 1));
                base.VisitBlockStmt(x);
                ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(x.Span.End - 1, 1));
            }
        }

        public override void VisitBoolLiteral(BoolLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitCaseItem(CaseItem x)
        {
            ConsumeToken(Tokens.T_CASE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4));
            VisitElement(x.CaseVal);
            ConsumeToken(Tokens.T_COLON, SpanUtils.SafeSpan(x.Span.End - 1, 1));

            base.VisitCaseItem(x);
        }

        public override void VisitCatchItem(CatchItem x)
        {
            // catch (TYPE VARIABLE) BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_CATCH, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));
                ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.TargetType.Span));
                VisitElement(x.TargetType);
                VisitElement(x.Variable);
                ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.Variable.Span, x.Body.Span));

                VisitElement(x.Body);
            }
        }

        public sealed override void VisitClassConstantDecl(ClassConstantDecl x)
        {
            VisitConstantDecl(x);
        }

        public override void VisitClassConstUse(ClassConstUse x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.NamePosition));
            ConsumeToken(Tokens.T_STRING, x.Name.Value, x.NamePosition);
        }

        public override void VisitClassTypeRef(ClassTypeRef x)
        {
            VisitQualifiedName(x.ClassName, x.Span);
        }

        public override void VisitConcatEx(ConcatEx x)
        {
            if (x.Label.Length > 1)
            {
                ConsumeToken(Tokens.T_START_HEREDOC, "<<<" + x.Label, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 0));
            }
            else if (x.Label.Length == 1)
            {
                ConsumeToken((Tokens)x.Label[0], SpanUtils.SafeSpan(x.Span.StartOrInvalid, 0));
            }
            VisitElementList(x.Expressions, Tokens.T_DOT, ".");
            if (x.Label.Length > 1)
            {
                ConsumeToken(Tokens.T_END_HEREDOC, x.Label, SpanUtils.SafeSpan(x.Span.End - x.Label.Length, 0));
            }
            else if (x.Label.Length == 1)
            {
                ConsumeToken((Tokens)x.Label[0], SpanUtils.SafeSpan(x.Span.End - x.Label.Length, 0));
            }
            object obj;
            if (x.Properties.TryGetProperty(ConcatEx.DelimitersPosition, out obj) &&
                obj is KeyValuePair<Tokens, short>[])
            {
                var delimiters = (KeyValuePair<Tokens, short>[])obj;
                for (int i = 0; i < delimiters.Length; i++)
                {
                    if (delimiters[i].Key != Tokens.T_ERROR)
                    {
                        ConsumeToken(delimiters[i].Key, SpanUtils.SafeSpan(x.Span.Start + delimiters[i].Value, 0));
                    }
                }
            }
        }

        public override void VisitConditionalEx(ConditionalEx x)
        {
            VisitElement(x.CondExpr);
            ConsumeToken(Tokens.T_QUESTION, "?", SpanUtils.SafeSpan(x.QuestionPosition, 0));
            VisitElement(x.TrueExpr);   // can be null
            ConsumeToken(Tokens.T_COLON, ":", SpanUtils.SafeSpan(x.ColonPosition, 0));
            VisitElement(x.FalseExpr);
        }

        public sealed override void VisitConditionalStmt(ConditionalStmt x)
        {
            throw new InvalidOperationException();  // VisitIfStmt
        }

        public override void VisitConstantDecl(ConstantDecl x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span);

            if (x.Initializer != null)  // always true
            {
                ConsumeToken(Tokens.T_EQ, "=", SpanUtils.SafeSpan(x.AssignmentPosition, 0));
                VisitElement(x.Initializer);
            }
        }

        public sealed override void VisitConstantUse(ConstantUse x)
        {
            throw new InvalidOperationException(); // override PseudoConstant, ClassCOnstant, GlobalConstant
        }

        public override void VisitConstDeclList(ConstDeclList x)
        {
            ConsumeModifiers(x, x.Modifiers, x.Span.IsValid && x.Constants[0].Span.IsValid ? Span.Combine(x.Span, x.Constants[0].Span) : Span.Invalid);
            ConsumeToken(Tokens.T_CONST, "const", SpanUtils.SafeSpan(x.ConstPosition, 0));
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitCustomAttribute(CustomAttribute x)
        {
            throw new NotImplementedException();
        }

        public override void VisitDeclareStmt(DeclareStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitDefaultItem(DefaultItem x)
        {
            ConsumeToken(Tokens.T_DEFAULT, "default", x.Span);
            ConsumeToken(Tokens.T_COLON, ":", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 7, 0));
            VisitList(x.Statements);
        }

        public override void VisitDirectFcnCall(DirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitQualifiedName(x.FullName.OriginalName, x.NameSpan);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectStFldUse(DirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", SpanUtils.SafeSpan(x.NameSpan.StartOrInvalid - 2, 0));
            VisitVariableName(x.PropertyName, x.NameSpan, true);  // $name
        }

        public override void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", SpanUtils.SafeSpan(x.MethodName.Span.StartOrInvalid - 2, 0));
            ConsumeToken(Tokens.T_STRING, x.MethodName.Name.Value, x.MethodName.Span);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectVarUse(DirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitVariableName(x.VarName, x.Span, x.IsMemberOf == null &&
                !(x.ContainingElement is DollarBracesExpression) &&
                !(x.ContainingElement is ItemUse &&
                x == ((ItemUse)x.ContainingElement).Array &&
                x.ContainingElement.ContainingElement is DollarBracesExpression));
        }

        public override void VisitDoubleLiteral(DoubleLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitEchoStmt(EchoStmt x)
        {
            if (x.IsHtmlCode)
            {
                ConsumeToken(Tokens.T_INLINE_HTML, ((StringLiteral)x.Parameters[0]).Value, x.Span);
            }
            else
            {
                // echo PARAMETERS;
                ConsumeToken(Tokens.T_ECHO, "echo", x.Span);
                VisitElementList(x.Parameters, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
            }
        }

        public override void VisitEncapsedExpression(EncapsedExpression x)
        {
            ConsumeToken(x.OpenToken, x.Span);
            VisitElement(x.Expression);
            ConsumeToken(x.CloseToken, SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitEmptyEx(EmptyEx x)
        {
            // empty(OPERAND)
            ConsumeToken(Tokens.T_EMPTY, "empty", x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 5, 0));
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitEmptyStmt(EmptyStmt x)
        {
            ConsumeToken(Tokens.T_SEMI, ";", x.Span);
        }

        public override void VisitEvalEx(EvalEx x)
        {
            ConsumeToken(Tokens.T_EVAL, "eval", x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 4, 0));
            VisitElement(x.Code);
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitExitEx(ExitEx x)
        {
            ConsumeToken(Tokens.T_EXIT, "exit", x.Span);
            VisitElement(x.ResulExpr);
        }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            base.VisitExpressionStmt(x);
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitFieldDecl(FieldDecl x)
        {
            VisitVariableName(x.Name, x.NameSpan, true);
            if (x.Initializer != null)
            {
                ConsumeToken(Tokens.T_EQ, "=", SpanUtils.SafeSpan(x.AssignOperatorPosition, 0));
                VisitElement(x.Initializer);
            }
        }

        public override void VisitFieldDeclList(FieldDeclList x)
        {
            ConsumeModifiers(x, x.Modifiers, x.Span.IsValid && x.Fields[0].Span.IsValid ? Span.Combine(x.Span, x.Fields[0].Span) : Span.Invalid);

            object value;
            if (x.Modifiers == 0 && !x.Properties.TryGetProperty(TypeMemberDecl.ModifierPositionProperty, out value))
            {
                ConsumeToken(Tokens.T_VAR, "var", x.Span);
            }

            VisitElementList(x.Fields, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            // finally BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FINAL, "finally", x.Span);
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FOREACH, "foreach", x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", x.ConditionPosition);
                VisitElement(x.Enumeree);
                ConsumeToken(Tokens.T_AS, "as", SpanUtils.SafeSpan(x.Enumeree.Span.End + 1, 0));
                if (x.KeyVariable != null)
                {
                    VisitForeachVar(x.KeyVariable);
                    ConsumeToken(Tokens.T_DOUBLE_ARROW, "=>", SpanUtils.SafeSpan(x.KeyVariable.Span.End + 1, 0));
                }
                VisitForeachVar(x.ValueVariable);
                ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.ConditionPosition.End - 1, 0));
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachVar(ForeachVar x)
        {
            if (x.Alias) ConsumeToken(Tokens.T_AMP, SpanUtils.SafeSpan(x.Span.StartOrInvalid - 1, 0));
            VisitElement(x.Target);
        }

        public override void VisitFormalParam(FormalParam x)
        {
            VisitElement(x.TypeHint);
            if (x.PassedByRef)
            {
                ConsumeToken(Tokens.T_AMP, "&", x.Span);
            }
            if (x.IsVariadic)
            {
                ConsumeToken(Tokens.T_ELLIPSIS, "...", x.Span);
            }

            VisitVariableName(x.Name.Name, x.Name.Span, true);
            if (x.InitValue != null)
            {
                ConsumeToken(Tokens.T_EQ, "=", SpanUtils.SafeSpan(x.AssignmentPosition, 0));
                VisitElement(x.InitValue);
            }
        }

        public override void VisitFormalTypeParam(FormalTypeParam x)
        {
            throw new NotImplementedException();
        }

        public override void VisitForStmt(ForStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FOR, "for", x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", x.ConditionPosition);

                VisitElementList(x.InitExList, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";", x.InitExList.Count > 0 ?
                    SpanUtils.SafeSpan(x.InitExList.Last().Span.End, 0) : SpanUtils.SafeSpan(x.ConditionPosition.StartOrInvalid + 2, 0));
                VisitElementList(x.CondExList, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";", x.CondExList.Count > 0 ?
                    SpanUtils.SafeSpan(x.CondExList.Last().Span.End, 0) : SpanUtils.SafeSpan(x.ConditionPosition.StartOrInvalid + 4, 0));
                VisitElementList(x.ActionExList, Tokens.T_COMMA, ",");

                ConsumeToken(Tokens.T_LPAREN, ")", SpanUtils.SafeSpan(x.ConditionPosition.End - 1, 0));

                VisitElement(x.Body);
            }
        }

        public sealed override void VisitFunctionCall(FunctionCall x)
        {
            throw new InvalidOperationException(); // DirectFncCall, IndirectFncCall, *MethodCall, ...
        }

        protected virtual void VisitRoutineDecl(LangElement element, Signature signature, Statement body, NameRef nameOpt = default(NameRef), TypeRef returnTypeOpt = null, PhpMemberAttributes modifiers = PhpMemberAttributes.None)
        {
            using (new ScopeHelper(this, element))
            {
                // function &NAME SIGNATURE : RETURN_TYPE BODY
                ConsumeModifiers(element, modifiers, element.Span.IsValid && nameOpt.Span.IsValid &&
                    element.Span.StartOrInvalid >= 0 && nameOpt.Span.StartOrInvalid > element.Span.StartOrInvalid ?
                    Span.Combine(element.Span, nameOpt.Span) : Span.Invalid);
                ConsumeToken(Tokens.T_FUNCTION, SpanUtils.SafeSpan(element is MethodDecl ? ((MethodDecl)element).FunctionPosition : element.Span.StartOrInvalid, 0));
                if (signature.AliasReturn && element is IAliasReturn) ConsumeToken(Tokens.T_AMP, "&", SpanUtils.SafeSpan(((IAliasReturn)element).ReferencePosition, 0));
                if (nameOpt.HasValue) ConsumeToken(Tokens.T_STRING, nameOpt.Name.Value, nameOpt.Span);
                VisitSignature(signature);
                if (returnTypeOpt != null)
                {
                    ConsumeToken(Tokens.T_COLON, ":", SpanUtils.SafeSpan(returnTypeOpt.SeparatorPosition, 0));
                    VisitElement(returnTypeOpt);
                }
                if (body != null)
                {
                    VisitElement(body);
                }
                else
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(element.Span.End - 1, 0));
                }
            }
        }

        public override void VisitFunctionDecl(FunctionDecl x)
        {
            VisitRoutineDecl(x, x.Signature, x.Body, x.Name, x.ReturnType);
        }

        public override void VisitGenericTypeRef(GenericTypeRef x)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobalCode(GlobalCode x)
        {
            base.VisitGlobalCode(x);
        }

        public sealed override void VisitGlobalConstantDecl(GlobalConstantDecl x)
        {
            VisitConstantDecl(x);
        }

        public override void VisitGlobalConstDeclList(GlobalConstDeclList x)
        {
            ConsumeToken(Tokens.T_CONST, "const", SpanUtils.SafeSpan(x.ConstPosition, 0));
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitGlobalConstUse(GlobalConstUse x)
        {
            VisitQualifiedName(x.FullName.OriginalName, x.Span);
        }

        public override void VisitGlobalStmt(GlobalStmt x)
        {
            ConsumeToken(Tokens.T_GLOBAL, x.Span);
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitGotoStmt(GotoStmt x)
        {
            ConsumeToken(Tokens.T_GOTO, "goto", x.Span);
            ConsumeToken(Tokens.T_STRING, x.LabelName.Name.Value, x.LabelName.Span);
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitHaltCompiler(HaltCompiler x)
        {
            ConsumeToken(Tokens.T_HALT_COMPILER, "__halt_compiler", x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.SignaturePosition);
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.SignaturePosition.End - 1, 0));
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitIfStmt(IfStmt x)
        {
            // if (cond) stmt [else if] [else]
            for (int i = 0; i < x.Conditions.Count; i++)
            {
                var cond = x.Conditions[i];
                using (new ScopeHelper(this, cond))
                {
                    if (i == 0)
                    {
                        ConsumeToken(Tokens.T_IF, cond.Span);
                    }
                    else if (cond.Condition != null)
                    {
                        ConsumeToken(Tokens.T_ELSEIF, cond.Span);
                    }
                    else
                    {
                        ConsumeToken(Tokens.T_ELSE, cond.Span);
                    }

                    if (cond.Condition != null)
                    {
                        ConsumeToken(Tokens.T_LPAREN, "(", cond.ConditionPosition);
                        VisitElement(cond.Condition);
                        ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(cond.ConditionPosition.End - 1, 0));
                    }

                    // TODO: ":" ?

                    VisitElement(cond.Statement);
                }
            }

            // TODO: ENDIF; ?
        }

        public override void VisitIncDecEx(IncDecEx x)
        {
            if (x.Post == true)
            {
                VisitElement(x.Variable);
            }

            // ++/--
            ConsumeToken(x.Inc ? Tokens.T_INC : Tokens.T_DEC, x.Inc ? "++" : "--", SpanUtils.SafeSpan(x.Post ? x.Span.End - 2 : x.Span.StartOrInvalid, 0));

            if (x.Post == false)
            {
                VisitElement(x.Variable);
            }
        }

        public override void VisitIncludingEx(IncludingEx x)
        {
            switch (x.InclusionType)
            {
                case InclusionTypes.Include:
                    ConsumeToken(Tokens.T_INCLUDE, "include", x.Span);
                    break;
                case InclusionTypes.IncludeOnce:
                    ConsumeToken(Tokens.T_INCLUDE_ONCE, "include_once", x.Span);
                    break;
                case InclusionTypes.Require:
                    ConsumeToken(Tokens.T_REQUIRE, "require", x.Span);
                    break;
                case InclusionTypes.RequireOnce:
                    ConsumeToken(Tokens.T_REQUIRE_ONCE, "require_once", x.Span);
                    break;

                default:
                    throw new ArgumentException();// ??
            }

            VisitElement(x.Target);
        }

        public override void VisitIndirectFcnCall(IndirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitElement(x.NameExpr);
            VisitCallSignature(x.CallSignature);
        }

        public virtual void VisitVariableName(VariableName name, Span span, bool dollar)
        {
            ConsumeToken(Tokens.T_VARIABLE, (dollar ? "$" : string.Empty) + name.Value, span);
        }

        public virtual void VisitQualifiedName(QualifiedName qname, Span span)
        {
            int position = span.StartOrInvalid;
            if (qname.ToString().Length + 1 < span.Length)
            {
                ConsumeToken(Tokens.T_NAMESPACE, SpanUtils.SafeSpan(position, 0));
                position += 9;
            }
            string separator = QualifiedName.Separator.ToString();
            if (qname.IsFullyQualifiedName && qname != QualifiedName.Null &&
                qname != QualifiedName.True && qname != QualifiedName.False)
            {
                ConsumeToken(Tokens.T_NS_SEPARATOR, separator, SpanUtils.SafeSpan(position, 0));
                position += separator.Length;
            }

            var ns = qname.Namespaces;
            for (int i = 0; i < ns.Length; i++)
            {
                ConsumeToken(Tokens.T_STRING, ns[i].Value, SpanUtils.SafeSpan(position, 0));
                position += ns[i].Value.Length;
                if (i != ns.Length - 1)
                {
                    ConsumeToken(Tokens.T_NS_SEPARATOR, separator, SpanUtils.SafeSpan(position, 0));
                    position += separator.Length;
                }
            }
            if (ns.Length > 0 && !string.IsNullOrEmpty(qname.Name.Value))
            {
                ConsumeToken(Tokens.T_NS_SEPARATOR, separator, SpanUtils.SafeSpan(position, 0));
                position += separator.Length;
            }
            ConsumeToken(Tokens.T_STRING, qname.Name.Value, SpanUtils.SafeSpan(position, 0));
        }

        public virtual void VisitCallSignature(CallSignature signature)
        {
            ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(signature.Position.Start, 0));
            VisitElementList(signature.Parameters, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(signature.Position.End - 1, 0));
        }

        public virtual void VisitSignature(Signature signature)
        {
            ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(signature.Span.Start, 0));
            VisitElementList(signature.FormalParams, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(signature.Span.End - 1, 0));
        }

        public override void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", SpanUtils.SafeSpan(x.TargetType.Span.End, 0));
            ConsumeToken(Tokens.T_DOLLAR, SpanUtils.SafeSpan(x.FieldNameExpr.Span.StartOrInvalid - 1, 0));
            VisitElement(x.FieldNameExpr);
        }

        public override void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", SpanUtils.SafeSpan(x.TargetType.Span.End, 0));
            VisitElement(x.MethodNameExpression);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitIndirectTypeRef(IndirectTypeRef x)
        {
            VisitElement(x.ClassNameVar);
        }

        public override void VisitIndirectVarUse(IndirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            if (x.IsMemberOf == null && !(x.ContainingElement is DollarBracesExpression))
            {
                ConsumeToken(Tokens.T_DOLLAR, x.Span);
            }
            VisitElement(x.VarNameEx);
        }

        public override void VisitInstanceOfEx(InstanceOfEx x)
        {
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_INSTANCEOF, "instanceof", SpanUtils.SafeSpan(x.OperatorPosition, 0));
            VisitElement(x.ClassNameRef);
        }

        public override void VisitIssetEx(IssetEx x)
        {
            ConsumeToken(Tokens.T_ISSET, "isset", x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 5, 0));
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitItemUse(ItemUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitElement(x.Array);
            ConsumeToken(x.IsBraces ? Tokens.T_LBRACE : Tokens.T_LBRACKET, SpanUtils.SafeSpan(x.Array.Span.End, 0));
            VisitElement(x.Index);
            ConsumeToken(x.IsBraces ? Tokens.T_RBRACE : Tokens.T_RBRACKET, SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public virtual void VisitIsMemberOf(Expression isMemberOf)
        {
            if (isMemberOf != null)
            {
                VisitElement(isMemberOf);
                ConsumeToken(Tokens.T_OBJECT_OPERATOR, "->", SpanUtils.SafeSpan(isMemberOf.Span.End, 0));
            }
        }

        public override void VisitJumpStmt(JumpStmt x)
        {
            switch (x.Type)
            {
                case JumpStmt.Types.Return:
                    ConsumeToken(Tokens.T_RETURN, "return", x.Span);
                    break;
                case JumpStmt.Types.Continue:
                    ConsumeToken(Tokens.T_CONTINUE, "continue", x.Span);
                    break;
                case JumpStmt.Types.Break:
                    ConsumeToken(Tokens.T_BREAK, "break", x.Span);
                    break;
            }

            VisitElement(x.Expression);

            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitLabelStmt(LabelStmt x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span);
            ConsumeToken(Tokens.T_COLON, ":", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitLambdaFunctionExpr(LambdaFunctionExpr element)
        {
            using (new ScopeHelper(this, element))
            {
                if (element.FunctionPosition == element.Span.Start)
                {
                    ConsumeToken(Tokens.T_FUNCTION, SpanUtils.SafeSpan(element.FunctionPosition, 0));
                }
                else
                {
                    ConsumeToken(Tokens.T_STATIC, SpanUtils.SafeSpan(element.Span.Start, 0));
                    ConsumeToken(Tokens.T_FUNCTION, SpanUtils.SafeSpan(element.FunctionPosition, 0));
                }
                if (element.Signature.AliasReturn && element is IAliasReturn) ConsumeToken(Tokens.T_AMP, "&", SpanUtils.SafeSpan(((IAliasReturn)element).ReferencePosition, 0));
                VisitSignature(element.Signature);
                if (element.UseParams != null)
                {
                    ConsumeToken(Tokens.T_USE, SpanUtils.SafeSpan(element.UsePosition, 0));
                    ConsumeToken(Tokens.T_LPAREN, SpanUtils.SafeSpan(element.UseSignaturePosition.StartOrInvalid, 0));
                    VisitElementList(element.UseParams, Tokens.T_COMMA, ",");
                    ConsumeToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(element.UseSignaturePosition.End - 1, 0));
                }
                if (element.ReturnType != null)
                {
                    ConsumeToken(Tokens.T_COLON, ":", SpanUtils.SafeSpan(element.ReturnType.SeparatorPosition, 0));
                    VisitElement(element.ReturnType);
                }
                if (element.Body != null)
                {
                    VisitElement(element.Body);
                }
                else
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(element.Span.End - 1, 0));
                }
            }
        }

        public override void VisitListEx(ListEx x)
        {
            if (x.IsOldNotation)
            {
                ConsumeToken(Tokens.T_LIST, "list", x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 4, 0));
                VisitElementList(x.Items, t => VisitArrayItem((Item)t), Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.Span.End - 1, 0));
            }
            else
            {
                ConsumeToken(Tokens.T_LBRACKET, x.Span);
                VisitElementList(x.Items, t => VisitArrayItem((Item)t), Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_RBRACKET, SpanUtils.SafeSpan(x.Span.End - 1, 0));
            }
        }

        protected virtual void VisitElementList<TElement>(IList<TElement> list, Tokens separatorToken, string separatorTokenText) where TElement : LangElement, ISeparatedElements
        {
            Debug.Assert(list != null, nameof(list));

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ConsumeToken(separatorToken, separatorTokenText, SpanUtils.SafeSpan(list[i - 1].SeparatorPosition, 0));
                VisitElement(list[i]);
            }
        }

        protected virtual void VisitElementList(IList<ISeparatedElements> list, Action<ISeparatedElements> action, Tokens separatorToken, string separatorTokenText)
        {
            Debug.Assert(list != null, nameof(list));
            Debug.Assert(action != null, nameof(action));

            for (int i = 0; i < list.Count; i++)
            {
                action(list[i]);
                //if (list[i].SeparatorPosition > 0)
                {
                    ConsumeToken(separatorToken, separatorTokenText, SpanUtils.SafeSpan(list[i].SeparatorPosition, 0));
                }
            }
        }

        private void VisitNamedTypeRef(INamedTypeRef tref) => VisitElement((TypeRef)tref);

        public override void VisitLongIntLiteral(LongIntLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitMethodDecl(MethodDecl x)
        {
            VisitRoutineDecl(x, x.Signature, x.Body, x.Name, x.ReturnType, x.Modifiers);
        }

        public override void VisitMultipleTypeRef(MultipleTypeRef x)
        {
            VisitElementList(x.MultipleTypes, (Tokens)'|', "|");
        }

        public override void VisitNamedActualParam(NamedActualParam x)
        {
            throw new NotImplementedException();
        }

        public override void VisitNamedTypeDecl(NamedTypeDecl x)
        {
            VisitTypeDecl(x);
        }

        public override void VisitNamespaceDecl(NamespaceDecl x)
        {
            ConsumeToken(Tokens.T_NAMESPACE, "namespace", x.Span);

            if (x.QualifiedName.HasValue)
            {
                var qname = x.QualifiedName.QualifiedName.WithFullyQualified(false);
                VisitQualifiedName(qname, x.QualifiedName.Span);
            }

            if (x.IsSimpleSyntax)
            {
                // namespace QNAME; BODY
                ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
                VisitList(x.Body.Statements);
            }
            else
            {
                // namespace QNAME { BODY }
                VisitElement(x.Body);
            }
        }

        public override void VisitNewEx(NewEx x)
        {
            ConsumeToken(Tokens.T_NEW, "new", x.Span);
            VisitElement(x.ClassNameRef);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitNullableTypeRef(NullableTypeRef x)
        {
            ConsumeToken(Tokens.T_QUESTION, "?", x.Span);
            VisitElement(x.TargetType);
        }

        public override void VisitNullLiteral(NullLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitPHPDocBlock(PHPDocBlock x)
        {
            // ignore
        }

        public override void VisitPHPDocStmt(PHPDocStmt x)
        {
            base.VisitPHPDocStmt(x);
        }

        public override void VisitPrimitiveTypeRef(PrimitiveTypeRef x)
        {
            VisitQualifiedName(x.QualifiedName.Value, x.Span);
        }

        public override void VisitPseudoClassConstUse(PseudoClassConstUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", SpanUtils.SafeSpan(x.TargetType.Span.End, 0));
            switch (x.Type)
            {
                case PseudoClassConstUse.Types.Class:
                    ConsumeToken(Tokens.T_CLASS, "class", x.NamePosition);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public override void VisitPseudoConstUse(PseudoConstUse x)
        {
            ConsumeToken(TokenFacts.GetPseudoConstUseToken(x.Type), x.Span);
        }

        public override void VisitRefAssignEx(RefAssignEx x)
        {
            // L =& R
            VisitElement(x.LValue);
            ConsumeToken(Tokens.T_EQ, "=", SpanUtils.SafeSpan(x.OperatorPosition, 0));
            ConsumeToken(Tokens.T_AMP, "&", SpanUtils.SafeSpan(x.RValue.Span.StartOrInvalid - 1, 0));
            VisitElement(x.RValue);
        }

        public sealed override void VisitRefItem(RefItem x)
        {
            throw new InvalidOperationException(); // VisitArrayItem
        }

        public override void VisitReservedTypeRef(ReservedTypeRef x)
        {
            VisitQualifiedName(x.QualifiedName.Value, x.Span);
        }

        public override void VisitShellEx(ShellEx x)
        {
            VisitElement(x.Command);
        }

        public override void VisitStaticStmt(StaticStmt x)
        {
            ConsumeToken(Tokens.T_STATIC, "static", x.Span);
            VisitElementList(x.StVarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitStaticVarDecl(StaticVarDecl x)
        {
            VisitVariableName(x.Variable, x.NameSpan, true);

            if (x.Initializer != null)
            {
                ConsumeToken(Tokens.T_EQ, "=", SpanUtils.SafeSpan(x.AssignmentPosition, 0));
                VisitElement(x.Initializer);
            }
        }

        public override void VisitStringLiteral(StringLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitStringLiteralDereferenceEx(StringLiteralDereferenceEx x)
        {
            VisitElement(x.StringExpr);
            ConsumeToken(Tokens.T_LBRACKET, "[", SpanUtils.SafeSpan(x.KeyExpr.Span.Start - 1, 0));
            VisitElement(x.KeyExpr);
            ConsumeToken(Tokens.T_RBRACE, "]", SpanUtils.SafeSpan(x.KeyExpr.Span.End, 0));
        }

        public sealed override void VisitSwitchItem(SwitchItem x)
        {
            throw new NotImplementedException();  // VisitDefaultItem, VisitCaseItem
        }

        public override void VisitSwitchStmt(SwitchStmt x)
        {
            // switch(VALUE){CASES}
            ConsumeToken(Tokens.T_SWITCH, "switch", x.Span);
            // TODO: ENDSWITCH or { }
            ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(x.SwitchValue.Span.StartOrInvalid - 1, 0));
            VisitElement(x.SwitchValue);
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.SwitchValue.Span.End, 0));
            ConsumeToken(Tokens.T_LBRACE, "{", SpanUtils.SafeSpan(x.SwitchValue.Span.End + 1, 0));
            VisitList(x.SwitchItems);
            ConsumeToken(Tokens.T_RBRACE, "}", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitThrowStmt(ThrowStmt x)
        {
            // throw EXPR;
            ConsumeToken(Tokens.T_THROW, "throw", x.Span);
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitTraitAdaptationAlias(TraitsUse.TraitAdaptationAlias x)
        {
            throw new NotImplementedException();
        }

        public override void VisitTraitAdaptationBlock(TraitAdaptationBlock x)
        {
            throw new NotImplementedException();
        }

        public override void VisitTraitAdaptationPrecedence(TraitsUse.TraitAdaptationPrecedence x)
        {
            throw new NotImplementedException();
        }

        public override void VisitTraitsUse(TraitsUse x)
        {
            throw new NotImplementedException();
            //ConsumeToken(Tokens.T_USE, x.Span);
            //foreach (var item in x.TraitsList)
            //{
            //    if (item.QualifiedName.HasValue)
            //    {
            //        ConsumeToken(Tokens.T_STRING, item.QualifiedName.Value.Name.Value, item.Span);
            //    }
            //}
            //ConsumeToken(Tokens.T_SEMI, ";",SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitTranslatedTypeRef(TranslatedTypeRef x)
        {
            VisitElement(x.OriginalType);
        }

        public override void VisitTryStmt(TryStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_TRY, "try", x.Span);
                VisitElement(x.Body);
                VisitList(x.Catches);
                VisitElement(x.FinallyItem);
            }
        }

        public override void VisitTypeDecl(TypeDecl x)
        {
            using (new ScopeHelper(this, x))
            {
                // final class|interface|trait [NAME] extends ... implements ... { MEMBERS }
                ConsumeModifiers(x, x.MemberAttributes, x.Span.IsValid && x.Name.Span.IsValid ?
                    Span.FromBounds(x.Span.Start, x.Name.Span.Start) : Span.Invalid);
                if ((x.MemberAttributes & PhpMemberAttributes.Interface) != 0) ConsumeToken(Tokens.T_INTERFACE, "interface", SpanUtils.SafeSpan(x.KeyworPosition, 0));
                else if ((x.MemberAttributes & PhpMemberAttributes.Trait) != 0) ConsumeToken(Tokens.T_TRAIT, "trait", SpanUtils.SafeSpan(x.KeyworPosition, 0));
                else ConsumeToken(Tokens.T_CLASS, "class", SpanUtils.SafeSpan(x.KeyworPosition, 0));

                if (x.Name.HasValue && !x.Name.Name.IsGenerated)
                {
                    ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span);
                }

                if (x.BaseClass != null)
                {
                    // extends
                    ConsumeToken(Tokens.T_EXTENDS, "extends", SpanUtils.SafeSpan(x.BaseClass.Span.StartOrInvalid - 8, 0));
                    VisitElement((TypeRef)x.BaseClass);
                }

                if (x.ImplementsList != null && x.ImplementsList.Length != 0)
                {
                    // implements|extends
                    if ((x.MemberAttributes & PhpMemberAttributes.Interface) == 0)
                    {
                        ConsumeToken(Tokens.T_IMPLEMENTS, SpanUtils.SafeSpan(x.ImplementsList[0].Span.StartOrInvalid - 11, 0));
                    }
                    else
                    {
                        ConsumeToken(Tokens.T_EXTENDS, SpanUtils.SafeSpan(x.ImplementsList[0].Span.StartOrInvalid - 8, 0));
                    }

                    VisitElementList(x.ImplementsList, t => VisitNamedTypeRef((INamedTypeRef)t), Tokens.T_COMMA, ",");
                }


                ConsumeToken(Tokens.T_LBRACE, "{", SpanUtils.SafeSpan(x.BodySpan.StartOrInvalid, 0));
                VisitList(x.Members);
                ConsumeToken(Tokens.T_RBRACE, "}", SpanUtils.SafeSpan(x.BodySpan.End - 1, 0));
            }
        }

        public override void VisitTypeOfEx(TypeOfEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnaryEx(UnaryEx x)
        {
            ConsumeToken(TokenFacts.GetOperationToken(x.Operation), x.Span);
            VisitElement(x.Expr);
        }

        public override void VisitUnsetStmt(UnsetStmt x)
        {
            ConsumeToken(Tokens.T_UNSET, "unset", x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 5, 0));
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.Span.End - 2, 0));
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitUseStatement(UseStatement x)
        {
            ConsumeToken(Tokens.T_USE, "use", x.Span);
            switch (x.Kind)
            {
                case AliasKind.Constant: ConsumeToken(Tokens.T_CONST, "const", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 4, 0)); break;
                case AliasKind.Function: ConsumeToken(Tokens.T_FUNCTION, "function", SpanUtils.SafeSpan(x.Span.StartOrInvalid + 4, 0)); break;
            }

            VisitElementList(x.Uses, t => VisitUse((UseBase)t), Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        protected virtual void VisitUse(UseBase use)
        {
            throw new NotImplementedException();
        }

        public override void VisitValueAssignEx(ValueAssignEx x)
        {
            // L = R
            VisitElement(x.LValue);
            switch (x.Operation)
            {
                case Operations.AssignAdd:
                    ConsumeToken(Tokens.T_PLUS_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignSub:
                    ConsumeToken(Tokens.T_MINUS_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignMul:
                    ConsumeToken(Tokens.T_MUL_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignPow:
                    ConsumeToken(Tokens.T_POW_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignDiv:
                    ConsumeToken(Tokens.T_DIV_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignMod:
                    ConsumeToken(Tokens.T_MOD_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignAnd:
                    ConsumeToken(Tokens.T_AND_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignOr:
                    ConsumeToken(Tokens.T_OR_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignXor:
                    ConsumeToken(Tokens.T_XOR_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignShiftLeft:
                    ConsumeToken(Tokens.T_SL_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignShiftRight:
                    ConsumeToken(Tokens.T_SR_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                case Operations.AssignAppend:
                    ConsumeToken(Tokens.T_CONCAT_EQUAL, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
                default:
                    ConsumeToken(Tokens.T_EQ, SpanUtils.SafeSpan(x.OperatorPosition, 0)); break;
            }
            VisitElement(x.RValue);
        }

        public sealed override void VisitValueItem(ValueItem x)
        {
            throw new InvalidOperationException();  // VisitArrayItem
        }

        public sealed override void VisitVarLikeConstructUse(VarLikeConstructUse x)
        {
            throw new InvalidOperationException();  // visit specific AST element
        }

        public override void VisitWhileStmt(WhileStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                if (x.LoopType == WhileStmt.Type.While)
                {
                    ConsumeToken(Tokens.T_WHILE, "while", x.Span);
                    ConsumeToken(Tokens.T_LPAREN, "(", x.ConditionPosition);
                    VisitElement(x.CondExpr);
                    ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.ConditionPosition.End - 1, 0));
                    VisitElement(x.Body);
                }
                else
                {
                    ConsumeToken(Tokens.T_DO, x.Span);
                    VisitElement(x.Body);
                    ConsumeToken(Tokens.T_WHILE, SpanUtils.SafeSpan(x.WhilePosition, 0));
                    ConsumeToken(Tokens.T_LPAREN, "(", x.ConditionPosition);
                    VisitElement(x.CondExpr);
                    ConsumeToken(Tokens.T_RPAREN, ")", SpanUtils.SafeSpan(x.ConditionPosition.End - 1, 0));
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 0));
                }
            }
        }

        public override void VisitYieldEx(YieldEx x)
        {
            ConsumeToken(Tokens.T_YIELD, "yield", x.Span);

            if (x.KeyExpr != null)
            {
                VisitElement(x.KeyExpr);
                ConsumeToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SafeSpan(x.OperatorPosition, 0));
            }

            VisitElement(x.ValueExpr);
        }

        public override void VisitYieldFromEx(YieldFromEx x)
        {
            ConsumeToken(Tokens.T_YIELD_FROM, x.Span);
            VisitElement(x.ValueExpr);
        }

        #endregion
    }
}
