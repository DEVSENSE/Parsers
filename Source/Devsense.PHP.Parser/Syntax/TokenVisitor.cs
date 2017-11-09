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
        /// <param name="tokens">Modifier tokens.</param>
        /// <param name="span">Optional. Modifiers span.</param>
        void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers,
            ISourceToken[] tokens, Span span = default(Span));

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
        public virtual void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, ISourceToken[] tokens, Span span)
        {
            foreach (var item in tokens)
            {
                ConsumeToken(item.Token, item.Span);
            }
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

        private ISourceToken ProcessToken(Tokens target, Span span)
        {
            var token = _provider.GetTokenAt(span, target, new SourceToken(target, Span.Invalid));
            ConsumeToken(token.Token, token.Span);
            return token;
        }

        private ISourceToken ProcessToken(Tokens target, string text, Span span)
        {
            var token = _provider.GetTokenAt(span, target, new SourceToken(target, Span.Invalid));
            ConsumeToken(token.Token, text, token.Span);
            return token;
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


        private static void AddPublicModifier(PhpMemberAttributes modifiers,
            Dictionary<Tokens, ISourceToken> defaults)
        {
            var token = PhpMemberAttributes.Public.ToToken();
            if ((modifiers & PhpMemberAttributes.VisibilityMask) == PhpMemberAttributes.Public)
                defaults.Add(token, new SourceToken(token, Span.Invalid));
        }

        private static void AddModifier(PhpMemberAttributes modifiers,
            PhpMemberAttributes modifier, Dictionary<Tokens, ISourceToken> defaults)
        {
            var token = modifier.ToToken();
            if ((modifiers & modifier) != 0)
                defaults.Add(token, new SourceToken(token, Span.Invalid));
        }

        protected ISourceToken[] ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            var defaults = new Dictionary<Tokens, ISourceToken>();
            AddPublicModifier(modifiers, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Private, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Protected, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Static, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Abstract, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Final, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Interface, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Trait, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Constructor, defaults);
            var tokens = _provider.GetTokens(span, t => defaults.ContainsKey(t.Token), defaults.Values).AsArray();
            _composer.ConsumeModifiers(element, modifiers, tokens, span);
            return tokens;
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
            var terminalSpan = itemSpan.IsValid ? SpanUtils.SpanIntermission(itemSpan, x.Span.End) : x.Span;
            if (x.IsOldNotation)
            {
                ProcessToken(Tokens.T_ARRAY, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span);
                var previous = ProcessToken(Tokens.T_LPAREN, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span);
                VisitItemList(x.Items, Tokens.T_COMMA, previous, terminalSpan);
                ProcessToken(Tokens.T_RPAREN, terminalSpan);
            }
            else
            {
                var previous = ProcessToken(Tokens.T_LBRACKET, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span);
                VisitItemList(x.Items, Tokens.T_COMMA, previous, terminalSpan);
                ProcessToken(Tokens.T_RBRACKET, terminalSpan);
            }
        }

        protected virtual void VisitItemList(IList<Item> list, Tokens separatorToken, ISourceToken previous, Span terminal)
        {
            Debug.Assert(list != null, nameof(list));
            for (int i = 0; i < list.Count; i++)
            {
                VisitArrayItem(list[i], previous);
                if (i + 1 != list.Count)
                {
                    previous = ProcessToken(separatorToken, list[i + 1] != null ?
                        SpanUtils.SpanIntermission(list[i] != null ? list[i].ItemSpan() : previous.Span, list[i + 1].ItemSpan()) :
                        terminal);
                }
            }
        }

        public void VisitArrayItem(Item item, ISourceToken previous)
        {
            if (item != null)
            {
                var valueSpan = item is ValueItem ? ((ValueItem)item).ValueExpr.Span : ((RefItem)item).RefToGet.Span;
                if (item.Index != null)
                {
                    VisitElement(item.Index);
                    previous = ProcessToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SpanIntermission(item.Index.Span, valueSpan));
                }

                if (item is ValueItem)
                {
                    VisitElement(((ValueItem)item).ValueExpr);
                }
                else if (item is RefItem)
                {
                    ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(
                        item.HasKey ? item.Index.Span : previous.Span, valueSpan));
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
            ConsumeToken(Tokens.T_STRING, "assert", SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6));
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.CodeEx.Span));
            VisitElement(x.CodeEx);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.CodeEx.Span, x.Span.End));
        }

        public sealed override void VisitAssignEx(AssignEx x) { throw new InvalidOperationException(); }



        public override void VisitBinaryEx(BinaryEx x)
        {
            VisitElement(x.LeftExpr);
            if (x.Operation == Operations.And)
            {
                ConsumeLogicalOperator(x.OperatorSpan, Tokens.T_BOOLEAN_AND, Tokens.T_LOGICAL_AND);
            }
            else if (x.Operation == Operations.Or)
            {
                ConsumeLogicalOperator(x.OperatorSpan, Tokens.T_BOOLEAN_OR, Tokens.T_LOGICAL_OR);
            }
            else
            {
                ProcessToken(TokenFacts.GetOperationToken(x.Operation), x.OperatorSpan);
            }
            VisitElement(x.RightExpr);
        }

        private void ConsumeLogicalOperator(Span span, Tokens symbolic, Tokens verbose)
        {
            ISourceToken token = new SourceToken(symbolic, Span.Invalid);
            var tokens = _provider.GetTokens(span, t => t.Token == symbolic || t.Token == verbose, new[] { token });
            if (tokens.Count() == 1)
            {
                token = tokens.Single();
            }
            ConsumeToken(token.Token, token.Span);
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
            var label = x.OpenLabel;
            ConsumeToken(x.OpenToken, label, SpanUtils.SafeSpan(x.Span.StartOrInvalid, label.Length));
            VisitElementList(x.Expressions, Tokens.T_DOT);
            label = x.CloseLabel;
            ConsumeToken(x.CloseToken, label, SpanUtils.SafeSpan(x.Span.End - label.Length, label.Length));
        }

        public override void VisitConditionalEx(ConditionalEx x)
        {
            VisitElement(x.CondExpr);
            ProcessToken(Tokens.T_QUESTION, SpanUtils.SpanIntermission(x.CondExpr.Span,
                x.TrueExpr != null ? x.TrueExpr.Span : x.FalseExpr.Span));
            VisitElement(x.TrueExpr);   // can be null
            ProcessToken(Tokens.T_COLON, SpanUtils.SpanIntermission(
                x.TrueExpr != null ? x.TrueExpr.Span : x.CondExpr.Span, x.FalseExpr.Span));
            VisitElement(x.FalseExpr);
        }

        public sealed override void VisitConditionalStmt(ConditionalStmt x)
        {
            throw new InvalidOperationException();  // VisitIfStmt
        }

        public override void VisitConstantDecl(ConstantDecl x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span);
            ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.Name.Span, x.Initializer.Span));
            VisitElement(x.Initializer);
        }

        public sealed override void VisitConstantUse(ConstantUse x)
        {
            throw new InvalidOperationException(); // override PseudoConstant, ClassCOnstant, GlobalConstant
        }

        public override void VisitConstDeclList(ConstDeclList x)
        {
            var constSpan = x.Constants == null || x.Constants.Count == 0 ? x.Span :
                SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Constants[0].Span);
            ConsumeModifiers(x, x.Modifiers, constSpan);
            ProcessToken(Tokens.T_CONST, constSpan);
            VisitElementList(x.Constants, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
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
            var labelSpan = x.Statements == null || x.Statements.Length == 0 ?
                x.Span : SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Statements[0].Span);
            ProcessToken(Tokens.T_DEFAULT, labelSpan);
            ProcessToken(Tokens.T_COLON, labelSpan);
            VisitList(x.Statements);
        }

        public override void VisitDirectFcnCall(DirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.NameSpan);
            VisitQualifiedName(x.FullName.OriginalName, x.NameSpan);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectStFldUse(DirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.NameSpan));
            VisitVariableName(x.PropertyName, x.NameSpan, true);  // $name
        }

        public override void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.MethodName.Span));
            ConsumeToken(Tokens.T_STRING, x.MethodName.Name.Value, x.MethodName.Span);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectVarUse(DirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.Span);
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
                ConsumeToken(Tokens.T_ECHO, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4));
                VisitElementList(x.Parameters, Tokens.T_COMMA);
                ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
            }
        }

        public override void VisitEncapsedExpression(EncapsedExpression x)
        {
            var text = TokenFacts.GetTokenText(x.OpenToken);
            ConsumeToken(x.OpenToken, text, SpanUtils.SafeSpan(x.Span.StartOrInvalid, text.Length));
            VisitElement(x.Expression);
            text = TokenFacts.GetTokenText(x.CloseToken);
            ConsumeToken(x.CloseToken, text, SpanUtils.SafeSpan(x.Span.End - text.Length, text.Length));
        }

        public override void VisitEmptyEx(EmptyEx x)
        {
            var emptySpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5);
            ConsumeToken(Tokens.T_EMPTY, emptySpan);
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(emptySpan, x.Expression.Span));
            VisitElement(x.Expression);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.Expression.Span, x.Span.End));
        }

        public override void VisitEmptyStmt(EmptyStmt x)
        {
            ConsumeToken(Tokens.T_SEMI, x.Span);
        }

        public override void VisitEvalEx(EvalEx x)
        {
            var evalSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4);
            ConsumeToken(Tokens.T_EVAL, evalSpan);
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(evalSpan, x.Code.Span));
            VisitElement(x.Code);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.Code.Span, x.Span.End));
        }

        public override void VisitExitEx(ExitEx x)
        {
            var exitSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4);
            ConsumeToken(Tokens.T_EXIT, exitSpan);
            var paren = _provider.GetTokenAt(x.ResulExpr != null ? SpanUtils.SpanIntermission(exitSpan, x.ResulExpr.Span) : x.Span, Tokens.T_LPAREN, null);
            if (paren != null)
            {
                ConsumeToken(paren.Token, paren.Span);
            }
            VisitElement(x.ResulExpr);
            paren = _provider.GetTokenAt(x.ResulExpr != null ? SpanUtils.SpanIntermission(x.ResulExpr.Span, x.Span.End) : x.Span, Tokens.T_RPAREN, null);
            if (paren != null)
            {
                ConsumeToken(paren.Token, paren.Span);
            }
        }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            base.VisitExpressionStmt(x);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitFieldDecl(FieldDecl x)
        {
            VisitVariableName(x.Name, x.NameSpan, true);
            if (x.Initializer != null)
            {
                ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.NameSpan, x.Initializer.Span));
                VisitElement(x.Initializer);
            }
        }

        public override void VisitFieldDeclList(FieldDeclList x)
        {
            var varSpan = x.Fields == null || x.Fields.Count == 0 ? x.Span :
                SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Fields[0].Span);
            var modifiers = ConsumeModifiers(x, x.Modifiers, varSpan);
            if (modifiers.Length == 0)
            {
                ProcessToken(Tokens.T_VAR, varSpan);
            }
            VisitElementList(x.Fields, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            // finally BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FINALLY, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7));
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                var foreachSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7);
                ConsumeToken(Tokens.T_FOREACH, foreachSpan);
                ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(foreachSpan, x.Enumeree.Span));
                VisitElement(x.Enumeree);
                ProcessToken(Tokens.T_AS, SpanUtils.SpanIntermission(x.Enumeree.Span,
                    x.KeyVariable != null ? x.KeyVariable.Span : x.ValueVariable.Span));
                if (x.KeyVariable != null)
                {
                    VisitForeachVar(x.KeyVariable);
                    ProcessToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SpanIntermission(x.KeyVariable.Span, x.ValueVariable.Span));
                }
                VisitForeachVar(x.ValueVariable);
                ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.ValueVariable.Span, x.Body.Span));
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachVar(ForeachVar x)
        {
            if (x.Alias)
            {
                ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(x.Span.StartOrInvalid - 1, x.Target.Span));
            }
            VisitElement(x.Target);
        }

        public override void VisitFormalParam(FormalParam x)
        {
            VisitElement(x.TypeHint);
            var modifierSpan = SpanUtils.SpanIntermission(
                x.TypeHint != null ? x.TypeHint.Span.StartOrInvalid : x.Span.StartOrInvalid, x.Name.Span);
            if (x.PassedByRef)
            {
                ProcessToken(Tokens.T_AMP, modifierSpan);
            }
            if (x.IsVariadic)
            {
                ProcessToken(Tokens.T_ELLIPSIS, modifierSpan);
            }

            VisitVariableName(x.Name.Name, x.Name.Span, true);
            if (x.InitValue != null)
            {
                ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.Name.Span, x.InitValue.Span));
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
                var foreachSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3);
                ConsumeToken(Tokens.T_FOR, foreachSpan);
                ProcessToken(Tokens.T_LPAREN, SpanUtils.SafeSpan(x.ConditionSpan.StartOrInvalid, 1));

                VisitElementList(x.InitExList, Tokens.T_COMMA);
                var previous = ProcessToken(Tokens.T_SEMI, x.ConditionSpan);
                VisitElementList(x.CondExList, Tokens.T_COMMA);
                ProcessToken(Tokens.T_SEMI, SpanUtils.SpanIntermission(previous.Span, x.ConditionSpan.End));
                VisitElementList(x.ActionExList, Tokens.T_COMMA);

                ProcessToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(x.ConditionSpan.End - 1, 1));
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
                var prenameSpan = SpanUtils.SpanIntermission(element.Span.StartOrInvalid, nameOpt.HasValue ? nameOpt.Span : signature.Span);
                ConsumeModifiers(element, modifiers, prenameSpan);
                ProcessToken(Tokens.T_FUNCTION, prenameSpan);
                if (signature.AliasReturn)
                {
                    ProcessToken(Tokens.T_AMP, prenameSpan);
                }
                if (nameOpt.HasValue)
                {
                    ConsumeToken(Tokens.T_STRING, nameOpt.Name.Value, nameOpt.Span);
                }
                VisitSignature(signature);
                if (returnTypeOpt != null)
                {
                    ProcessToken(Tokens.T_COLON, SpanUtils.SpanIntermission(signature.Span,
                        body != null ? body.Span.StartOrInvalid : element.Span.End));
                    VisitElement(returnTypeOpt);
                }
                if (body != null)
                {
                    VisitElement(body);
                }
                else
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(element.Span.End - 1, 1));
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
            ConsumeToken(Tokens.T_CONST, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));
            VisitElementList(x.Constants, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitGlobalConstUse(GlobalConstUse x)
        {
            VisitQualifiedName(x.FullName.OriginalName, x.Span);
        }

        public override void VisitGlobalStmt(GlobalStmt x)
        {
            ConsumeToken(Tokens.T_GLOBAL, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6));
            VisitElementList(x.VarList, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitGotoStmt(GotoStmt x)
        {
            ConsumeToken(Tokens.T_GOTO, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4));
            ConsumeToken(Tokens.T_STRING, x.LabelName.Name.Value, x.LabelName.Span);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitHaltCompiler(HaltCompiler x)
        {
            ConsumeToken(Tokens.T_HALT_COMPILER, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 15));
            ProcessToken(Tokens.T_LPAREN, x.Span);
            ProcessToken(Tokens.T_RPAREN, x.Span);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
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
                        ConsumeToken(Tokens.T_IF, SpanUtils.SafeSpan(cond.Span.StartOrInvalid, 2));
                    }
                    else if (cond.Condition != null)
                    {
                        ConsumeToken(Tokens.T_ELSEIF, SpanUtils.SafeSpan(cond.Span.StartOrInvalid, 6));
                    }
                    else
                    {
                        ConsumeToken(Tokens.T_ELSE, SpanUtils.SafeSpan(cond.Span.StartOrInvalid, 4));
                    }

                    if (cond.Condition != null)
                    {
                        ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(cond.Span.StartOrInvalid, cond.Condition.Span));
                        VisitElement(cond.Condition);
                        ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(cond.Condition.Span, cond.Statement.Span));
                    }

                    VisitElement(cond.Statement);
                }
            }
        }

        public override void VisitIncDecEx(IncDecEx x)
        {
            if (x.Post == true)
            {
                VisitElement(x.Variable);
            }

            // ++/--
            ConsumeToken(x.Inc ? Tokens.T_INC : Tokens.T_DEC, x.Inc ? "++" : "--", SpanUtils.SafeSpan(x.Post ? x.Span.End - 2 : x.Span.StartOrInvalid, 2));

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
                    ConsumeToken(Tokens.T_INCLUDE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7));
                    break;
                case InclusionTypes.IncludeOnce:
                    ConsumeToken(Tokens.T_INCLUDE_ONCE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 12));
                    break;
                case InclusionTypes.Require:
                    ConsumeToken(Tokens.T_REQUIRE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7));
                    break;
                case InclusionTypes.RequireOnce:
                    ConsumeToken(Tokens.T_REQUIRE_ONCE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 12));
                    break;

                default:
                    throw new ArgumentException();// ??
            }

            VisitElement(x.Target);
        }

        public override void VisitIndirectFcnCall(IndirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.NameExpr.Span);
            VisitElement(x.NameExpr);
            VisitCallSignature(x.CallSignature);
        }

        public virtual void VisitVariableName(VariableName name, Span span, bool dollar)
        {
            var varname = (dollar ? "$" : string.Empty) + name.Value;
            ConsumeToken(Tokens.T_VARIABLE, varname, span);
        }

        private static int AddSeparato(List<ISourceToken> defaults, string separator, int position)
        {
            defaults.Add(new SourceToken(Tokens.T_NS_SEPARATOR, SpanUtils.SafeSpan(position, separator.Length)));
            return position + separator.Length;
        }

        private static List<ISourceToken> BuildQnameDefaults(QualifiedName qname, Span span)
        {
            string separator = QualifiedName.Separator.ToString();
            var defaults = new List<ISourceToken>();
            int position = span.StartOrInvalid;
            if (qname.IsFullyQualifiedName && qname != QualifiedName.Null &&
                qname != QualifiedName.True && qname != QualifiedName.False)
            {
                position = AddSeparato(defaults, separator, position);
            }

            var ns = qname.Namespaces;
            for (int i = 0; i < ns.Length; i++)
            {
                if (i != 0)
                {
                    position = AddSeparato(defaults, separator, position);
                }
                defaults.Add(new SourceToken(Tokens.T_STRING, SpanUtils.SafeSpan(position, ns[i].Value.Length)));
                position += ns[i].Value.Length;
            }
            if (ns.Length > 0 && !string.IsNullOrEmpty(qname.Name.Value))
            {
                position = AddSeparato(defaults, separator, position);
            }
            if (!string.IsNullOrEmpty(qname.Name.Value))
            {
                defaults.Add(new SourceToken(Tokens.T_STRING, SpanUtils.SafeSpan(position, qname.Name.Value.Length)));
            }
            return defaults;
        }

        public virtual void VisitQualifiedName(QualifiedName qname, Span span)
        {
            var defaults = BuildQnameDefaults(qname, span);
            var tokens = _provider.GetTokens(span, t => t.Token == Tokens.T_NAMESPACE ||
            t.Token == Tokens.T_NS_SEPARATOR || t.Token == Tokens.T_STRING || t.Token == Tokens.T_STATIC ||
            t.Token == Tokens.T_CALLABLE || t.Token == Tokens.T_ARRAY, defaults).ToArray();
            if (tokens.Length == defaults.Count)
            {
                ProcessQnameTokens(qname, tokens, 0);
            }
            else if (tokens.Length > 0 && tokens[0].Token == Tokens.T_NAMESPACE)
            {
                ProcessQnameTokens(qname, tokens, qname.Namespaces.Length - tokens.Count(t => t.Token == Tokens.T_STRING) + 1);
            }
        }

        private void ProcessQnameTokens(QualifiedName qname, ISourceToken[] tokens, int initNamespace)
        {
            int nsCount = initNamespace;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Token != Tokens.T_STRING)
                {
                    ConsumeToken(tokens[i].Token, tokens[i].Span);
                }
                else if (nsCount < qname.Namespaces.Length)
                {
                    ConsumeToken(tokens[i].Token, qname.Namespaces[nsCount++].Value, tokens[i].Span);
                }
                else
                {
                    ConsumeToken(tokens[i].Token, qname.Name.Value, tokens[i].Span);
                }
            }
        }

        public virtual void VisitCallSignature(CallSignature signature)
        {
            ConsumeToken(Tokens.T_LPAREN, SpanUtils.SafeSpan(signature.Position.StartOrInvalid, 1));
            VisitElementList(signature.Parameters, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(signature.Position.End - 1, 1));
        }

        public virtual void VisitSignature(Signature signature)
        {
            ConsumeToken(Tokens.T_LPAREN, SpanUtils.SafeSpan(signature.Span.StartOrInvalid, 1));
            VisitElementList(signature.FormalParams, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(signature.Span.End - 1, 1));
        }

        public override void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            VisitElement(x.TargetType);
            var interSpan = SpanUtils.SpanIntermission(x.TargetType.Span, x.FieldNameExpr.Span);
            ProcessToken(Tokens.T_DOUBLE_COLON, interSpan);
            ProcessToken(Tokens.T_DOLLAR, interSpan);
            VisitElement(x.FieldNameExpr);
        }

        public override void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.MethodNameExpression.Span));
            VisitElement(x.MethodNameExpression);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitIndirectTypeRef(IndirectTypeRef x)
        {
            VisitElement(x.ClassNameVar);
        }

        public override void VisitIndirectVarUse(IndirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.VarNameEx.Span);
            if (x.IsMemberOf == null && !(x.ContainingElement is DollarBracesExpression))
            {
                ProcessToken(Tokens.T_DOLLAR, x.Span);
            }
            VisitElement(x.VarNameEx);
        }

        public override void VisitInstanceOfEx(InstanceOfEx x)
        {
            VisitElement(x.Expression);
            ProcessToken(Tokens.T_INSTANCEOF, SpanUtils.SpanIntermission(x.Expression.Span, x.ClassNameRef.Span));
            VisitElement(x.ClassNameRef);
        }

        public override void VisitIssetEx(IssetEx x)
        {
            ConsumeToken(Tokens.T_ISSET, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));
            ProcessToken(Tokens.T_LPAREN, x.Span);
            VisitElementList(x.VarList, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitItemUse(ItemUse x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.Array.Span);
            VisitElement(x.Array);
            ProcessToken(x.IsBraces ? Tokens.T_LBRACE : Tokens.T_LBRACKET, SpanUtils.SpanIntermission(x.Array.Span,
                x.Index != null ? x.Index.Span.StartOrInvalid : x.Span.End));
            VisitElement(x.Index);
            ConsumeToken(x.IsBraces ? Tokens.T_RBRACE : Tokens.T_RBRACKET, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public virtual void VisitIsMemberOf(Expression isMemberOf, Span next)
        {
            if (isMemberOf != null)
            {
                VisitElement(isMemberOf);
                ProcessToken(Tokens.T_OBJECT_OPERATOR, SpanUtils.SpanIntermission(isMemberOf.Span, next));
            }
        }

        public override void VisitJumpStmt(JumpStmt x)
        {
            switch (x.Type)
            {
                case JumpStmt.Types.Return:
                    ConsumeToken(Tokens.T_RETURN, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6));
                    break;
                case JumpStmt.Types.Continue:
                    ConsumeToken(Tokens.T_CONTINUE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7));
                    break;
                case JumpStmt.Types.Break:
                    ConsumeToken(Tokens.T_BREAK, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));
                    break;
            }

            VisitElement(x.Expression);

            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitLabelStmt(LabelStmt x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span);
            ConsumeToken(Tokens.T_COLON, ":", SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitLambdaFunctionExpr(LambdaFunctionExpr element)
        {
            using (new ScopeHelper(this, element))
            {
                var initialSpan = SpanUtils.SpanIntermission(element.Span.StartOrInvalid, element.Signature.Span);
                ConsumeModifiers(element, element.Modifiers, initialSpan);
                ProcessToken(Tokens.T_FUNCTION, initialSpan);
                if (element.Signature.AliasReturn)
                {
                    ConsumeToken(Tokens.T_AMP, initialSpan);
                }
                VisitSignature(element.Signature);
                var useSpan = SpanUtils.SpanIntermission(element.Signature.Span,
                    element.ReturnType != null ? element.ReturnType.Span.StartOrInvalid : (element.Body != null ? element.Body.Span.StartOrInvalid : element.Span.End));
                if (element.UseParams != null)
                {
                    ProcessToken(Tokens.T_USE, useSpan);
                    ProcessToken(Tokens.T_LPAREN, useSpan);
                    VisitElementList(element.UseParams, Tokens.T_COMMA);
                    ProcessToken(Tokens.T_RPAREN, element.UseParams.Count > 0 ?
                        SpanUtils.SpanIntermission(element.UseParams.Last().Span, useSpan.End) : useSpan);
                }
                if (element.ReturnType != null)
                {
                    ProcessToken(Tokens.T_COLON, SpanUtils.SpanIntermission(element.Signature.Span,
                        element.Body != null ? element.Body.Span.StartOrInvalid : element.Span.End));
                    VisitElement(element.ReturnType);
                }
                if (element.Body != null)
                {
                    VisitElement(element.Body);
                }
                else
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(element.Span.End - 1, 1));
                }
            }
        }

        public override void VisitListEx(ListEx x)
        {
            var itemSpan = x.Items.ItemsSpan();
            var terminalSpan = itemSpan.IsValid ? SpanUtils.SpanIntermission(itemSpan, x.Span.End) : x.Span;
            if (x.IsOldNotation)
            {
                ConsumeToken(Tokens.T_LIST, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 1));
                var previous = ProcessToken(Tokens.T_LPAREN, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span);
                VisitItemList(x.Items, Tokens.T_COMMA, previous, terminalSpan);
                ProcessToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(x.Span.End - 1, 1));
            }
            else
            {
                var openSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 1);
                ConsumeToken(Tokens.T_LBRACKET, openSpan);
                VisitItemList(x.Items, Tokens.T_COMMA, new SourceToken(Tokens.T_LBRACKET, openSpan), terminalSpan);
                ConsumeToken(Tokens.T_RBRACKET, SpanUtils.SafeSpan(x.Span.End - 1, 1));
            }
        }

        protected virtual void VisitElementList<TElement>(IList<TElement> list, Tokens separatorToken) where TElement : LangElement
        {
            Debug.Assert(list != null, nameof(list));

            var separatorTokenText = TokenFacts.GetTokenText(separatorToken);
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ProcessToken(separatorToken, separatorTokenText,
                    SpanUtils.SpanIntermission(
                        list[i - 1] != null ? list[i - 1].Span : Span.Invalid,
                        list[i] != null ? list[i].Span : Span.Invalid));
                VisitElement(list[i]);
            }
        }

        protected virtual void VisitElementList(IList<UseBase> list, Tokens separatorToken)
        {
            // TODO - unify UseBase
            Debug.Assert(list != null, nameof(list));

            var separatorTokenText = TokenFacts.GetTokenText(separatorToken);
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ProcessToken(separatorToken, separatorTokenText,
                    SpanUtils.SpanIntermission(
                        list[i - 1] != null ? list[i - 1].Span : Span.Invalid,
                        list[i] != null ? list[i].Span : Span.Invalid));
                VisitUse(list[i]);
            }
        }

        protected virtual void VisitElementList(IList<INamedTypeRef> list, Tokens separatorToken)
        {
            // TODO - unify INamedTypeRef
            Debug.Assert(list != null, nameof(list));

            var separatorTokenText = TokenFacts.GetTokenText(separatorToken);
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ProcessToken(separatorToken, separatorTokenText,
                    SpanUtils.SpanIntermission(
                        list[i - 1] != null ? list[i - 1].Span : Span.Invalid,
                        list[i] != null ? list[i].Span : Span.Invalid));
                VisitNamedTypeRef(list[i]);
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
            VisitElementList(x.MultipleTypes, Tokens.T_PIPE);
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
            ConsumeToken(Tokens.T_NAMESPACE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 9));

            if (x.QualifiedName.HasValue)
            {
                var qname = x.QualifiedName.QualifiedName.WithFullyQualified(false);
                VisitQualifiedName(qname, x.QualifiedName.Span);
            }

            if (x.IsSimpleSyntax)
            {
                // namespace QNAME; BODY
                ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
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
            ConsumeToken(Tokens.T_NEW, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3));
            VisitElement(x.ClassNameRef);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitNullableTypeRef(NullableTypeRef x)
        {
            ConsumeToken(Tokens.T_QUESTION, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 1));
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
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.Span.End));
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
            ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.LValue.Span, x.RValue.Span));
            ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(x.LValue.Span, x.RValue.Span));
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
            ConsumeToken(Tokens.T_STATIC, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6));
            VisitElementList(x.StVarList, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitStaticVarDecl(StaticVarDecl x)
        {
            VisitVariableName(x.Variable, x.NameSpan, true);

            if (x.Initializer != null)
            {
                ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.NameSpan, x.Initializer.Span));
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
            ProcessToken(Tokens.T_LBRACKET, SpanUtils.SpanIntermission(x.StringExpr.Span, x.KeyExpr.Span));
            VisitElement(x.KeyExpr);
            ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public sealed override void VisitSwitchItem(SwitchItem x)
        {
            throw new NotImplementedException();  // VisitDefaultItem, VisitCaseItem
        }

        public override void VisitSwitchStmt(SwitchStmt x)
        {
            // switch(VALUE){CASES}
            ConsumeToken(Tokens.T_SWITCH, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6));
            // TODO: ENDSWITCH or { }
            ProcessToken(Tokens.T_LPAREN, x.Span);
            VisitElement(x.SwitchValue);
            var braceSpan = SpanUtils.SpanIntermission(x.SwitchValue.Span,
                x.SwitchItems.Length > 0 ? x.SwitchItems.First().Span.StartOrInvalid : x.Span.End);
            ProcessToken(Tokens.T_RPAREN, braceSpan);
            ProcessToken(Tokens.T_LBRACE, braceSpan);
            VisitList(x.SwitchItems);
            ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitThrowStmt(ThrowStmt x)
        {
            // throw EXPR;
            ConsumeToken(Tokens.T_THROW, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
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
            //ConsumeToken(Tokens.T_SEMI,SpanUtils.SafeSpan(x.Span.End - 1, 0));
        }

        public override void VisitTranslatedTypeRef(TranslatedTypeRef x)
        {
            VisitElement(x.OriginalType);
        }

        public override void VisitTryStmt(TryStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_TRY, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3));
                VisitElement(x.Body);
                VisitList(x.Catches);
                VisitElement(x.FinallyItem);
            }
        }

        public override void VisitTypeDecl(TypeDecl x)
        {
            using (new ScopeHelper(this, x))
            {
                var lastSpan = SpanUtils.SafeSpan(x.BodySpan.End - 1, 1);
                var bodySpan = x.Members.Count != 0 ? x.Members.First().Span : lastSpan;
                var implementsSpan = x.ImplementsList != null && x.ImplementsList.Length != 0 ? x.ImplementsList.First().Span : bodySpan;
                var baseSpan = x.BaseClass != null ? x.BaseClass.Span : implementsSpan;
                var nameSpan = x.Name.HasValue && !x.Name.Name.IsGenerated ? x.Name.Span : baseSpan;
                var prenameSpan = SpanUtils.SpanIntermission(x.Span.StartOrInvalid, nameSpan);

                // final class|interface|trait [NAME] extends ... implements ... { MEMBERS }
                var previous = ConsumeModifiers(x, x.MemberAttributes, prenameSpan).LastOrDefault();
                if ((x.MemberAttributes & PhpMemberAttributes.Interface) == 0 && (x.MemberAttributes & PhpMemberAttributes.Trait) == 0)
                {
                    previous = ProcessToken(Tokens.T_CLASS, prenameSpan);
                }
                if (x.Name.HasValue && !x.Name.Name.IsGenerated)
                {
                    previous = ProcessToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span);
                }
                if (x.BaseClass != null)
                {
                    // extends
                    previous = ProcessToken(Tokens.T_EXTENDS, SpanUtils.SpanIntermission(previous.Span, baseSpan));
                    VisitElement((TypeRef)x.BaseClass);
                }
                if (x.ImplementsList != null && x.ImplementsList.Length != 0)
                {
                    // implements|extends
                    if ((x.MemberAttributes & PhpMemberAttributes.Interface) == 0)
                    {
                        previous = ProcessToken(Tokens.T_IMPLEMENTS, SpanUtils.SpanIntermission(previous.Span, implementsSpan));
                    }
                    else
                    {
                        previous = ProcessToken(Tokens.T_EXTENDS, SpanUtils.SpanIntermission(previous.Span, implementsSpan));
                    }

                    VisitElementList(x.ImplementsList, Tokens.T_COMMA);
                }

                ProcessToken(Tokens.T_LBRACE, SpanUtils.SpanIntermission(previous.Span, bodySpan));
                VisitList(x.Members);
                ConsumeToken(Tokens.T_RBRACE, lastSpan);
            }
        }

        public override void VisitTypeOfEx(TypeOfEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnaryEx(UnaryEx x)
        {
            var token = TokenFacts.GetOperationToken(x.Operation);
            var text = TokenFacts.GetTokenText(token);
            ConsumeToken(token, text, SpanUtils.SafeSpan(x.Span.StartOrInvalid, text.Length));
            VisitElement(x.Expr);
        }

        public override void VisitUnsetStmt(UnsetStmt x)
        {
            ConsumeToken(Tokens.T_UNSET, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));
            ProcessToken(Tokens.T_LPAREN, x.Span);
            VisitElementList(x.VarList, Tokens.T_COMMA);
            ProcessToken(Tokens.T_RPAREN, x.VarList.Count > 0 ? SpanUtils.SpanIntermission(x.VarList.Last().Span, x.Span.End) : x.Span);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        public override void VisitUseStatement(UseStatement x)
        {
            ConsumeToken(Tokens.T_USE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3));
            switch (x.Kind)
            {
                case AliasKind.Constant: ProcessToken(Tokens.T_CONST, x.Span); break;
                case AliasKind.Function: ProcessToken(Tokens.T_FUNCTION, x.Span); break;
            }

            VisitElementList(x.Uses, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
        }

        protected virtual void VisitUse(UseBase use)
        {
            throw new NotImplementedException();
        }

        public override void VisitValueAssignEx(ValueAssignEx x)
        {
            // L = R
            VisitElement(x.LValue);
            ProcessToken(TokenFacts.GetOperationToken(x.Operation), SpanUtils.SpanIntermission(x.LValue.Span, x.RValue.Span));
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
                    ConsumeToken(Tokens.T_WHILE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));
                    ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.Start, x.CondExpr.Span));
                    VisitElement(x.CondExpr);
                    ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.CondExpr.Span, x.Body.Span));
                    VisitElement(x.Body);
                }
                else
                {
                    ConsumeToken(Tokens.T_DO, x.Span);
                    VisitElement(x.Body);
                    Span whileSpan = SpanUtils.SpanIntermission(x.Body.Span, x.CondExpr.Span);
                    ProcessToken(Tokens.T_WHILE, whileSpan);
                    ProcessToken(Tokens.T_LPAREN, whileSpan);
                    VisitElement(x.CondExpr);
                    ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.CondExpr.Span, x.Span.End));
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1));
                }
            }
        }

        public override void VisitYieldEx(YieldEx x)
        {
            ConsumeToken(Tokens.T_YIELD, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5));

            if (x.KeyExpr != null)
            {
                VisitElement(x.KeyExpr);
                ProcessToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SpanIntermission(x.KeyExpr.Span, x.ValueExpr.Span));
            }

            VisitElement(x.ValueExpr);
        }

        public override void VisitYieldFromEx(YieldFromEx x)
        {
            ConsumeToken(Tokens.T_YIELD_FROM, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 10));
            VisitElement(x.ValueExpr);
        }

        #endregion
    }
}
