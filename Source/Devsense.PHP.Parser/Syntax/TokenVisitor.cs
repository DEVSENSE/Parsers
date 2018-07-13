using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax.Ast;
using System.Diagnostics;
using System.Globalization;
using Devsense.PHP.Text;
using static Devsense.PHP.Syntax.Ast.EncapsedExpression;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Options specifying how <see cref="TokenVisitor"/> synthesizes tokens from the syntax tree if not specified.
    /// </summary>
    public interface ITokenComposer
    {
        /// <summary>
        /// Consumes next token.
        /// </summary>
        /// <param name="token">The token ID.</param>
        /// <param name="text">Token source code content - synthesized or passed from original source code.</param>
        /// <param name="position">Original token position in source code.</param>
        /// <param name="sourceElement">Source AST element.</param>
        void ConsumeToken(Tokens token, string text, Span position, LangElement sourceElement);
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
        /// Shortcut for <see cref="ConsumeToken(Tokens, string, Span, LangElement)"/>.
        /// </summary>
        protected void ConsumeToken(Tokens token, Span position, LangElement sourceElement) => ConsumeToken(token, TokenFacts.GetTokenText(token), position, sourceElement);

        /// <inheritdoc />
        public virtual void ConsumeToken(Tokens token, string text, Span position, LangElement sourceElement)
        {
            // to be overwritten in derived class
            Debug.WriteLine("ConsumeToken {0}: {1}", token.ToString(), text);
        }
    }

    #endregion

    public class TokenVisitor : TreeContextVisitor
    {
        private readonly ITokenComposer _composer;
        private readonly ISourceTokenProvider _provider;

        /// <summary>
        /// Consumes a literal.
        /// Calls corresponding <see cref="ITokenComposer.ConsumeToken"/>.
        /// </summary>
        public virtual void ConsumeLiteral(Literal literal)
        {
            if (literal is BoolLiteral blit)
            {
                ConsumeToken(Tokens.T_STRING, literal.SourceText ?? (blit.Value ? "true" : "false"), literal.Span, literal);
            }
            else if (literal is DoubleLiteral dlit)
            {
                ConsumeToken(Tokens.T_DNUMBER, literal.SourceText ?? dlit.Value.ToString(CultureInfo.InvariantCulture), literal.Span, literal);
            }
            else if (literal is NullLiteral)
            {
                ConsumeToken(Tokens.T_STRING, literal.SourceText ?? "null", literal.Span, literal);
            }
            else if (literal is LongIntLiteral)
            {
                bool isArrayItemInConcat = literal.ContainingElement is ItemUse && literal.ContainingElement.ContainingElement is ConcatEx;
                ConsumeToken(
                    isArrayItemInConcat ? Tokens.T_NUM_STRING : Tokens.T_LNUMBER,
                    literal.SourceText ?? ((LongIntLiteral)literal).Value.ToString(CultureInfo.InvariantCulture),
                    literal.Span, literal);
            }
            else if (literal is StringLiteral slit)
            {
                if (literal.ContainingElement is ItemUse && literal.ContainingElement.ContainingElement is ConcatEx)
                {
                    ConsumeToken(Tokens.T_STRING, literal.SourceText ?? slit.Value, literal.Span, literal);
                }
                else if (literal.ContainingElement is ShellEx)
                {
                    Debug.Assert(literal.SourceText == null || literal.SourceText.Length >= 2);
                    var value = literal.SourceText != null ? literal.SourceText.Substring(1, literal.SourceText.Length - 2) : slit.Value;
                    ConsumeToken(Tokens.T_BACKQUOTE, SpanUtils.SafeSpan(literal.Span.StartOrInvalid, 1), literal);
                    if (value.Length != 0)
                    {
                        ConsumeToken(
                            Tokens.T_ENCAPSED_AND_WHITESPACE,
                            value,
                            literal.Span.IsValid ? SpanUtils.SafeSpan(literal.Span.Start + 1, literal.Span.Length - 2) : Span.Invalid, literal);
                    }
                    ConsumeToken(Tokens.T_BACKQUOTE, SpanUtils.SafeSpan(literal.Span.End - 1, 1), literal);
                }
                else
                {
                    if (literal.Span.Length != 0)   // literal must exist or invalid
                    {
                        ConsumeToken(
                            (literal.ContainingElement is ConcatEx || (literal.ContainingElement is StringEncapsedExpression stre && stre.OpenToken == Tokens.T_START_HEREDOC))
                                ? Tokens.T_ENCAPSED_AND_WHITESPACE
                                : Tokens.T_CONSTANT_ENCAPSED_STRING,
                            literal.SourceText ?? $"\"{slit.Value}\"",
                            literal.Span, literal);
                    }
                }
            }
        }

        protected virtual bool ExitHasParentheses { get { return false; } }
        protected virtual bool SwitchShortNotation { get { return true; } }

        /// <summary>
        /// Consume modifier tokens.
        /// Calls corresponding <see cref="ITokenComposer.ConsumeToken"/>.
        /// </summary>
        /// <param name="element">Original declaration element.</param>
        /// <param name="modifiers">Modifiers.</param>
        /// <param name="tokens">Modifier tokens.</param>
        /// <param name="span">Optional. Modifiers span.</param>
        protected virtual void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, ISourceToken[] tokens, Span span)
        {
            tokens.Foreach(t => ConsumeToken(t, element));
        }

        private ISourceToken ProcessToken(Tokens target, Span span, LangElement sourceNode)
        {
            var token = _provider.GetTokenAt(span, target, new SourceToken(target, Span.Invalid));
            ConsumeToken(token, sourceNode);
            return token;
        }

        private ISourceToken ProcessToken(Tokens target, string text, Span span, LangElement sourceNode)
        {
            var token = _provider.GetTokenAt(span, target, new SourceToken(target, Span.Invalid));
            ConsumeToken(token.Token, text, token.Span, sourceNode);
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
        /// <param name="sourceElement">Optional. Source AST element.</param>
        public void ConsumeToken(Tokens token, string text, Span position, LangElement sourceElement)
        {
            _composer.ConsumeToken(token, text, position, sourceElement);
        }

        private static void AddPublicModifier(PhpMemberAttributes modifiers,
            Dictionary<Tokens, ISourceToken> defaults)
        {
            var token = PhpMemberAttributes.Public.AsToken();
            if ((modifiers & PhpMemberAttributes.VisibilityMask) == PhpMemberAttributes.Public)
                defaults.Add(token, new SourceToken(token, Span.Invalid));
        }

        private static void AddModifier(PhpMemberAttributes modifiers,
            PhpMemberAttributes modifier, Dictionary<Tokens, ISourceToken> defaults)
        {
            var token = modifier.AsToken();
            if ((modifiers & modifier) != 0)
                defaults.Add(token, new SourceToken(token, Span.Invalid));
        }

        protected virtual ISourceToken[] ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            var defaults = new Dictionary<Tokens, ISourceToken>();
            if (element is MethodDecl || element is FieldDeclList || element is ConstDeclList)
            {
                AddPublicModifier(modifiers, defaults);
            }
            AddModifier(modifiers, PhpMemberAttributes.Private, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Protected, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Static, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Abstract, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Final, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Interface, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Trait, defaults);
            AddModifier(modifiers, PhpMemberAttributes.Constructor, defaults);
            var tokens = _provider.GetTokens(span, t => defaults.ContainsKey(t.Token), defaults.Values).AsArray();
            ConsumeModifiers(element, modifiers, tokens, span);
            return tokens;
        }

        /// <summary>
        /// Shortcut for <see cref="ConsumeToken(Tokens, string, Span, LangElement)"/>.
        /// </summary>
        protected void ConsumeToken(Tokens token, Span position, LangElement sourceNode) =>
            ConsumeToken(token, TokenFacts.GetTokenText(token), position, sourceNode);
        protected void ConsumeToken(ISourceToken token, LangElement sourceNode) =>
            ConsumeToken(token.Token, TokenFacts.GetTokenText(token.Token), token.Span, sourceNode);

        #region Single Nodes Overrides

        public override void VisitElement(LangElement element)
        {
            base.VisitElement(element);
        }

        public override void VisitActualParam(ActualParam x)
        {
            if (x.IsUnpack)
            {
                ProcessToken(Tokens.T_ELLIPSIS, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Expression.Span), x);
            }
            if (x.Ampersand)
            {
                ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Expression.Span), x);
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
            if (x.IsShortSyntax)
            {
                var previous = ProcessToken(Tokens.T_LBRACKET, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span, x);
                VisitItemList(x.Items, Tokens.T_COMMA, previous, terminalSpan, x);
                ProcessToken(Tokens.T_RBRACKET, terminalSpan, x);
            }
            else
            {
                ProcessToken(TokenFacts.GetOperationToken(x.Operation), itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span, x);
                var previous = ProcessToken(Tokens.T_LPAREN, itemSpan.IsValid ? SpanUtils.SpanIntermission(x.Span.StartOrInvalid, itemSpan) : x.Span, x);
                VisitItemList(x.Items, Tokens.T_COMMA, previous, terminalSpan, x);
                ProcessToken(Tokens.T_RPAREN, terminalSpan, x);
            }
        }

        public override void VisitListEx(ArrayEx x)
        {
            VisitArrayEx(x);
        }

        protected virtual void VisitItemList(IList<Item> list, Tokens separatorToken, ISourceToken previous, Span terminal, LangElement sourceNode)
        {
            Debug.Assert(list != null, nameof(list));
            for (int i = 0; i < list.Count; i++)
            {
                VisitArrayItem(list[i], previous, sourceNode);
                if (i + 1 != list.Count)
                {
                    previous = ProcessToken(separatorToken, list[i + 1] != null ?
                        SpanUtils.SpanIntermission(list[i] != null ? list[i].ItemSpan() : previous.Span, list[i + 1].ItemSpan()) :
                        terminal, sourceNode);
                }
            }
        }

        public void VisitArrayItem(Item item, ISourceToken previous, LangElement sourceNode)
        {
            if (item != null)
            {
                var valueSpan = item is ValueItem ? ((ValueItem)item).ValueExpr.Span : ((RefItem)item).RefToGet.Span;
                if (item.Index != null)
                {
                    VisitElement(item.Index);
                    previous = ProcessToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SpanIntermission(item.Index.Span, valueSpan), sourceNode);
                }

                if (item is ValueItem)
                {
                    VisitElement(((ValueItem)item).ValueExpr);
                }
                else if (item is RefItem)
                {
                    ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(
                        item.HasKey ? item.Index.Span : previous.Span, valueSpan), sourceNode);
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
            ConsumeToken(Tokens.T_STRING, "assert", SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6), x);
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.CodeEx.Span), x);
            VisitElement(x.CodeEx);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.CodeEx.Span, x.Span.End), x);
        }

        public sealed override void VisitAssignEx(AssignEx x) { throw new InvalidOperationException(); }

        public override void VisitBinaryEx(BinaryEx x)
        {
            VisitElement(x.LeftExpr);
            if (x.Operation == Operations.And)
            {
                ConsumeLogicalOperator(x.OperatorSpan, Tokens.T_BOOLEAN_AND, Tokens.T_LOGICAL_AND, x);
            }
            else if (x.Operation == Operations.Or)
            {
                ConsumeLogicalOperator(x.OperatorSpan, Tokens.T_BOOLEAN_OR, Tokens.T_LOGICAL_OR, x);
            }
            else
            {
                ProcessToken(TokenFacts.GetOperationToken(x.Operation), x.OperatorSpan, x);
            }
            VisitElement(x.RightExpr);
        }

        private void ConsumeLogicalOperator(Span span, Tokens symbolic, Tokens verbose, LangElement sourceNode)
        {
            ISourceToken token = new SourceToken(symbolic, Span.Invalid);
            var tokens = _provider.GetTokens(span, t => t.Token == symbolic || t.Token == verbose, new[] { token });
            if (tokens.Count() == 1)
            {
                token = tokens.Single();
            }
            ConsumeToken(token.Token, token.Span, sourceNode);
        }

        public override void VisitBinaryStringLiteral(BinaryStringLiteral x)
        {
            ConsumeLiteral(x);
        }

        public override void VisitBlockStmt(BlockStmt x)
        {
            if (x is ColonBlockStmt block)
            {
                var openingText = TokenFacts.GetTokenText(block.OpeningToken);
                ConsumeToken(block.OpeningToken, openingText, new Span(x.Span.StartOrInvalid, openingText.Length), x);
                base.VisitBlockStmt(x);
                var closingText = TokenFacts.GetTokenText(block.ClosingToken);
                ConsumeToken(block.ClosingToken, closingText, SpanUtils.SafeSpan(x.Span.End, closingText.Length), x);
                if (block.ClosingToken != Tokens.T_ELSE && block.ClosingToken != Tokens.T_ELSEIF)
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End + closingText.Length, 1), x);
                }
            }
            else
            {
                ConsumeToken(Tokens.T_LBRACE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 1), x);
                base.VisitBlockStmt(x);
                ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
            }
        }

        public override void VisitBoolLiteral(BoolLiteral x)
        {
            ConsumeLiteral(x);
        }

        public override void VisitCatchItem(CatchItem x)
        {
            // catch (TYPE VARIABLE) BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_CATCH, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);
                ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.TargetType.Span), x);
                VisitElement(x.TargetType);
                VisitElement(x.Variable);
                ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.Variable.Span, x.Body.Span), x);

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
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.NamePosition), x);
            ConsumeNameToken(x.Name.Value, x.NamePosition, x);
        }

        public override void VisitClassTypeRef(ClassTypeRef x)
        {
            VisitQualifiedName(x.ClassName, x.Span, x);
        }

        public override void VisitConcatEx(ConcatEx x)
        {
            VisitElementList(x.Expressions);
        }

        public override void VisitConditionalEx(ConditionalEx x)
        {
            VisitElement(x.CondExpr);
            ProcessToken(Tokens.T_QUESTION, SpanUtils.SpanIntermission(x.CondExpr.Span,
                x.TrueExpr != null ? x.TrueExpr.Span : x.FalseExpr.Span), x);
            VisitElement(x.TrueExpr);   // can be null
            ProcessToken(Tokens.T_COLON, SpanUtils.SpanIntermission(
                x.TrueExpr != null ? x.TrueExpr.Span : x.CondExpr.Span, x.FalseExpr.Span), x);
            VisitElement(x.FalseExpr);
        }

        public sealed override void VisitConditionalStmt(ConditionalStmt x)
        {
            throw new InvalidOperationException();  // VisitIfStmt
        }

        public override void VisitConstantDecl(ConstantDecl x)
        {
            ConsumeNameToken(x.Name.Name.Value, x.Name.Span, x);
            ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.Name.Span, x.Initializer.Span), x);
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
            ProcessToken(Tokens.T_CONST, constSpan, x);
            VisitElementList(x.Constants, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitDeclareStmt(DeclareStmt x)
        {
            ConsumeToken(Tokens.T_DECLARE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7), x);
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.ConstantDeclarations[0].Span), x);
            VisitElementList(x.ConstantDeclarations, Tokens.T_COMMA);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.ConstantDeclarations[x.ConstantDeclarations.Length - 1].Span, x.Statement.Span), x);
            VisitElement(x.Statement);
        }

        public override void VisitDirectFcnCall(DirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.NameSpan);
            VisitQualifiedName(x.FullName.OriginalName, x.NameSpan, x);
            VisitCallSignature(x.CallSignature, x);
        }

        public override void VisitDirectStFldUse(DirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.NameSpan), x);
            VisitVariableName(x.PropertyName, x.NameSpan, true, false, x);  // $name
        }

        public override void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.MethodName.Span), x);
            ConsumeNameToken(x.MethodName.Name.Value, x.MethodName.Span, x);
            VisitCallSignature(x.CallSignature, x);
        }

        public override void VisitDirectVarUse(DirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.Span);
            VisitVariableName(x.VarName, x.Span, x.IsMemberOf == null &&
                !(x.ContainingElement is DollarBracesExpression) &&
                !(x.ContainingElement is ItemUse &&
                x == ((ItemUse)x.ContainingElement).Array &&
                x.ContainingElement.ContainingElement is DollarBracesExpression), x.ContainingElement is DollarBracesExpression ||
                x.ContainingElement is ItemUse && x.ContainingElement.ContainingElement is DollarBracesExpression, x);
        }

        public override void VisitDoubleLiteral(DoubleLiteral x)
        {
            ConsumeLiteral(x);
        }

        public override void VisitEchoStmt(EchoStmt x)
        {
            if (x.IsHtmlCode)
            {
                ConsumeToken(Tokens.T_INLINE_HTML, ((StringLiteral)x.Parameters[0]).Value, x.Span, x);
            }
            else
            {
                // echo PARAMETERS;
                var echoSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4);
                var tokens = _provider.GetTokens(echoSpan, t => t.Token == Tokens.T_ECHO || t.Token == Tokens.T_OPEN_TAG_WITH_ECHO, null);
                var token = tokens == null || tokens.Count() != 1 ? new SourceToken(Tokens.T_ECHO, echoSpan) : tokens.Single();
                ConsumeToken(token.Token, token.Span, x);
                VisitElementList(x.Parameters, Tokens.T_COMMA);
                ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
            }
        }

        public override void VisitEncapsedExpression(EncapsedExpression x)
        {
            var text = x is StringEncapsedExpression ? ((StringEncapsedExpression)x).OpenLabel : TokenFacts.GetTokenText(x.OpenToken);
            ProcessToken(x.ContainingElement is ConcatEx && x is BracesExpression ? Tokens.T_CURLY_OPEN : x.OpenToken,
                text, SpanUtils.SpanIntermission(x.Span.StartOrInvalid, x.Expression.Span), x);
            VisitElement(x.Expression);
            text = x is StringEncapsedExpression ? ((StringEncapsedExpression)x).CloseLabel : TokenFacts.GetTokenText(x.CloseToken);
            ProcessToken(x.CloseToken, text, SpanUtils.SpanIntermission(x.Expression.Span, x.Span.End), x);
        }

        public override void VisitEmptyEx(EmptyEx x)
        {
            var emptySpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5);
            ConsumeToken(Tokens.T_EMPTY, emptySpan, x);
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(emptySpan, x.Expression.Span), x);
            VisitElement(x.Expression);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.Expression.Span, x.Span.End), x);
        }

        public override void VisitEmptyStmt(EmptyStmt x)
        {
            ConsumeToken(Tokens.T_SEMI, x.Span, x);
        }

        public override void VisitEvalEx(EvalEx x)
        {
            var evalSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4);
            ConsumeToken(Tokens.T_EVAL, evalSpan, x);
            ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(evalSpan, x.Code.Span), x);
            VisitElement(x.Code);
            ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.Code.Span, x.Span.End), x);
        }

        public override void VisitExitEx(ExitEx x)
        {
            var token = _provider.GetTokenAt(x.Span, Tokens.T_EXIT, new SourceToken(Tokens.T_EXIT, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4)));
            ConsumeToken(token.Token, _provider.GetTokenText(token, "exit"), token.Span, x);
            var paren = _provider.GetTokenAt(x.ResulExpr != null ? SpanUtils.SpanIntermission(token.Span, x.ResulExpr.Span) : x.Span, Tokens.T_LPAREN,
                ExitHasParentheses ? new SourceToken(Tokens.T_LPAREN, Span.Invalid) : null);
            if (paren != null)
            {
                ConsumeToken(paren.Token, paren.Span, x);
            }
            VisitElement(x.ResulExpr);
            paren = _provider.GetTokenAt(x.ResulExpr != null ? SpanUtils.SpanIntermission(x.ResulExpr.Span, x.Span.End) : x.Span, Tokens.T_RPAREN,
                ExitHasParentheses ? new SourceToken(Tokens.T_RPAREN, Span.Invalid) : null);
            if (paren != null)
            {
                ConsumeToken(paren.Token, paren.Span, x);
            }
        }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            base.VisitExpressionStmt(x);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitFieldDecl(FieldDecl x)
        {
            VisitVariableName(x.Name, x.NameSpan, true, false, x);
            if (x.Initializer != null)
            {
                ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.NameSpan, x.Initializer.Span), x);
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
                ProcessToken(Tokens.T_VAR, varSpan, x);
            }
            VisitElementList(x.Fields, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            // finally BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FINALLY, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7), x);
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                var foreachSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7);
                ConsumeToken(Tokens.T_FOREACH, foreachSpan, x);
                ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(foreachSpan, x.Enumeree.Span), x);
                VisitElement(x.Enumeree);
                ProcessToken(Tokens.T_AS, SpanUtils.SpanIntermission(x.Enumeree.Span,
                    x.KeyVariable != null ? x.KeyVariable.Span : x.ValueVariable.Span), x);
                if (x.KeyVariable != null)
                {
                    VisitForeachVar(x.KeyVariable);
                    ProcessToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SpanIntermission(x.KeyVariable.Span, x.ValueVariable.Span), x);
                }
                VisitForeachVar(x.ValueVariable);
                ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.ValueVariable.Span, x.Body.Span), x);
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachVar(ForeachVar x)
        {
            if (x.Alias)
            {
                ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(x.Span.StartOrInvalid - 1, x.Target.Span), x.Target);
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
                ProcessToken(Tokens.T_AMP, modifierSpan, x);
            }
            if (x.IsVariadic)
            {
                ProcessToken(Tokens.T_ELLIPSIS, modifierSpan, x);
            }

            VisitVariableName(x.Name.Name, x.Name.Span, true, false, x);
            if (x.InitValue != null)
            {
                ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.Name.Span, x.InitValue.Span), x);
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
                ConsumeToken(Tokens.T_FOR, foreachSpan, x);
                ProcessToken(Tokens.T_LPAREN, SpanUtils.SafeSpan(x.ConditionSpan.StartOrInvalid, 1), x);

                VisitElementList(x.InitExList, Tokens.T_COMMA);
                var previous = ProcessToken(Tokens.T_SEMI, x.ConditionSpan, x);
                VisitElementList(x.CondExList, Tokens.T_COMMA);
                ProcessToken(Tokens.T_SEMI, SpanUtils.SpanIntermission(previous.Span, x.ConditionSpan.End), x);
                VisitElementList(x.ActionExList, Tokens.T_COMMA);

                ProcessToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(x.ConditionSpan.End - 1, 1), x);
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
                ProcessToken(Tokens.T_FUNCTION, prenameSpan, element);
                if (signature.AliasReturn)
                {
                    ProcessToken(Tokens.T_AMP, prenameSpan, element);
                }
                if (nameOpt.HasValue)
                {
                    ConsumeNameToken(nameOpt.Name.Value, nameOpt.Span, element);
                }
                VisitSignature(signature, element);
                if (returnTypeOpt != null)
                {
                    ProcessToken(Tokens.T_COLON, SpanUtils.SpanIntermission(signature.Span,
                        body != null ? body.Span.StartOrInvalid : element.Span.End), element);
                    VisitElement(returnTypeOpt);
                }
                if (body != null)
                {
                    VisitElement(body);
                }
                else
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(element.Span.End - 1, 1), element);
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
            ConsumeToken(Tokens.T_CONST, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);
            VisitElementList(x.Constants, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitGlobalConstUse(GlobalConstUse x)
        {
            VisitQualifiedName(x.FullName.OriginalName, x.Span, x);
        }

        public override void VisitGlobalStmt(GlobalStmt x)
        {
            ConsumeToken(Tokens.T_GLOBAL, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6), x);
            VisitElementList(x.VarList, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitGotoStmt(GotoStmt x)
        {
            ConsumeToken(Tokens.T_GOTO, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4), x);
            ConsumeNameToken(x.LabelName.Name.Value, x.LabelName.Span, x);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitHaltCompiler(HaltCompiler x)
        {
            ConsumeToken(Tokens.T_HALT_COMPILER, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 15), x);
            ProcessToken(Tokens.T_LPAREN, x.Span, x);
            ProcessToken(Tokens.T_RPAREN, x.Span, x);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
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
                        ConsumeToken(Tokens.T_IF, SpanUtils.SafeSpan(cond.Span.StartOrInvalid, 2), x);
                    }
                    else if (!(x.Conditions[i - 1].Statement is ColonBlockStmt))
                    {
                        if (cond.Condition != null)
                        {
                            ConsumeToken(Tokens.T_ELSEIF, SpanUtils.SafeSpan(cond.Span.StartOrInvalid, 6), x);
                        }
                        else
                        {
                            ConsumeToken(Tokens.T_ELSE, SpanUtils.SafeSpan(cond.Span.StartOrInvalid, 4), x);
                        }
                    }

                    if (cond.Condition != null)
                    {
                        ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(cond.Span.StartOrInvalid, cond.Condition.Span), x);
                        VisitElement(cond.Condition);
                        ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(cond.Condition.Span, cond.Statement.Span), x);
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
            ConsumeToken(x.Inc ? Tokens.T_INC : Tokens.T_DEC, x.Inc ? "++" : "--", SpanUtils.SafeSpan(x.Post ? x.Span.End - 2 : x.Span.StartOrInvalid, 2), x);

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
                    ConsumeToken(Tokens.T_INCLUDE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7), x);
                    break;
                case InclusionTypes.IncludeOnce:
                    ConsumeToken(Tokens.T_INCLUDE_ONCE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 12), x);
                    break;
                case InclusionTypes.Require:
                    ConsumeToken(Tokens.T_REQUIRE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7), x);
                    break;
                case InclusionTypes.RequireOnce:
                    ConsumeToken(Tokens.T_REQUIRE_ONCE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 12), x);
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
            VisitCallSignature(x.CallSignature, x);
        }

        public virtual void VisitVariableName(VariableName name, Span span, bool dollar, bool isEncapsed, LangElement sourceNode)
        {
            var varname = (dollar ? "$" : string.Empty) + name.Value;
            ConsumeToken(dollar ? Tokens.T_VARIABLE : (isEncapsed ? Tokens.T_STRING_VARNAME : Tokens.T_STRING), varname, span, sourceNode);
        }

        private static int AddSeparator(List<ISourceToken> defaults, string separator, int position)
        {
            defaults.Add(new SourceToken(Tokens.T_NS_SEPARATOR, SpanUtils.SafeSpan(position, separator.Length)));
            return position + separator.Length;
        }

        private static List<ISourceToken> BuildQnameDefaults(QualifiedName qname, Span span, bool isNamespace)
        {
            string separator = QualifiedName.Separator.ToString();
            var defaults = new List<ISourceToken>();
            int position = span.StartOrInvalid;
            if (qname.IsFullyQualifiedName && qname != QualifiedName.Null &&
                qname != QualifiedName.True && qname != QualifiedName.False)
            {
                position = AddSeparator(defaults, separator, position);
            }

            var ns = qname.Namespaces;
            for (int i = 0; i < ns.Length; i++)
            {
                if (i != 0)
                {
                    position = AddSeparator(defaults, separator, position);
                }
                defaults.Add(new SourceToken(Tokens.T_STRING, SpanUtils.SafeSpan(position, ns[i].Value.Length)));
                position += ns[i].Value.Length;
            }
            if (ns.Length > 0 && !isNamespace)
            {
                position = AddSeparator(defaults, separator, position);
            }
            if (!string.IsNullOrEmpty(qname.Name.Value))
            {
                if (TokenFacts.s_reservedNameToToken.TryGetValue(qname.Name.Value, out Tokens t) == false || qname.Name.Value == "throw")
                {
                    t = Tokens.T_STRING;
                }
                defaults.Add(new SourceToken(t, SpanUtils.SafeSpan(position, qname.Name.Value.Length)));
            }
            return defaults;
        }

        public virtual void VisitQualifiedName(QualifiedName qname, Span span, LangElement sourceElement, bool isNamespace = false)
        {
            var defaults = BuildQnameDefaults(qname, span, isNamespace);
            var tokens = _provider.GetTokens(span, t => t.Token == Tokens.T_NAMESPACE ||
            t.Token == Tokens.T_NS_SEPARATOR || t.Token == Tokens.T_STRING || t.Token == Tokens.T_STATIC ||
            t.Token == Tokens.T_CALLABLE || t.Token == Tokens.T_ARRAY, defaults).ToArray();
            if (tokens.Length == defaults.Count)
            {
                ProcessQnameTokens(qname, tokens, 0, sourceElement);
            }
            else if (tokens.Length > 0 && tokens[0].Token == Tokens.T_NAMESPACE)
            {
                ProcessQnameTokens(qname, tokens, qname.Namespaces.Length - tokens.Count(t => t.Token == Tokens.T_STRING) + 1, sourceElement);
            }
        }

        private void ProcessQnameTokens(QualifiedName qname, ISourceToken[] tokens, int initNamespace, LangElement sourceElement)
        {
            int nsCount = initNamespace;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Token != Tokens.T_STRING)
                {
                    ConsumeToken(tokens[i].Token, tokens[i].Span, sourceElement);
                }
                else if (nsCount < qname.Namespaces.Length)
                {
                    ConsumeToken(tokens[i].Token, qname.Namespaces[nsCount++].Value, tokens[i].Span, sourceElement);
                }
                else
                {
                    ConsumeToken(tokens[i].Token, qname.Name.Value, tokens[i].Span, sourceElement);
                }
            }
        }

        public virtual void VisitCallSignature(CallSignature signature, LangElement sourceNode)
        {
            ConsumeToken(Tokens.T_LPAREN, SpanUtils.SafeSpan(signature.Position.StartOrInvalid, 1), sourceNode);
            VisitElementList(signature.Parameters, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(signature.Position.End - 1, 1), sourceNode);
        }

        public virtual void VisitSignature(Signature signature, LangElement sourceNode)
        {
            ConsumeToken(Tokens.T_LPAREN, SpanUtils.SafeSpan(signature.Span.StartOrInvalid, 1), sourceNode);
            VisitElementList(signature.FormalParams, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(signature.Span.End - 1, 1), sourceNode);
        }

        public override void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            VisitElement(x.TargetType);
            var interSpan = SpanUtils.SpanIntermission(x.TargetType.Span, x.FieldNameExpr.Span);
            ProcessToken(Tokens.T_DOUBLE_COLON, interSpan, x);
            ProcessToken(Tokens.T_DOLLAR, interSpan, x);
            VisitElement(x.FieldNameExpr);
        }

        public override void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.MethodNameExpression.Span), x);
            VisitElement(x.MethodNameExpression);
            VisitCallSignature(x.CallSignature, x);
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
                ProcessToken(Tokens.T_DOLLAR, x.Span, x);
            }
            VisitElement(x.VarNameEx);
        }

        public override void VisitInstanceOfEx(InstanceOfEx x)
        {
            VisitElement(x.Expression);
            ProcessToken(Tokens.T_INSTANCEOF, SpanUtils.SpanIntermission(x.Expression.Span, x.ClassNameRef.Span), x);
            VisitElement(x.ClassNameRef);
        }

        public override void VisitIssetEx(IssetEx x)
        {
            ConsumeToken(Tokens.T_ISSET, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);
            ProcessToken(Tokens.T_LPAREN, x.Span, x);
            VisitElementList(x.VarList, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_RPAREN, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitItemUse(ItemUse x)
        {
            VisitIsMemberOf(x.IsMemberOf, x.Array.Span);
            VisitElement(x.Array);
            ProcessToken(x.IsBraces ? Tokens.T_LBRACE : Tokens.T_LBRACKET, SpanUtils.SpanIntermission(x.Array.Span,
                x.Index != null ? x.Index.Span.StartOrInvalid : x.Span.End), x);
            VisitElement(x.Index);
            ConsumeToken(x.IsBraces ? Tokens.T_RBRACE : Tokens.T_RBRACKET, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public virtual void VisitIsMemberOf(Expression isMemberOf, Span next)
        {
            if (isMemberOf != null)
            {
                VisitElement(isMemberOf);
                ProcessToken(Tokens.T_OBJECT_OPERATOR, SpanUtils.SpanIntermission(isMemberOf.Span, next), isMemberOf);
            }
        }

        public override void VisitJumpStmt(JumpStmt x)
        {
            switch (x.Type)
            {
                case JumpStmt.Types.Return:
                    ConsumeToken(Tokens.T_RETURN, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6), x);
                    break;
                case JumpStmt.Types.Continue:
                    ConsumeToken(Tokens.T_CONTINUE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 8), x);
                    break;
                case JumpStmt.Types.Break:
                    ConsumeToken(Tokens.T_BREAK, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);
                    break;
            }

            VisitElement(x.Expression);

            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitLabelStmt(LabelStmt x)
        {
            ConsumeNameToken(x.Name.Name.Value, x.Name.Span, x);
            ConsumeToken(Tokens.T_COLON, ":", SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitLambdaFunctionExpr(LambdaFunctionExpr element)
        {
            using (new ScopeHelper(this, element))
            {
                var initialSpan = SpanUtils.SpanIntermission(element.Span.StartOrInvalid, element.Signature.Span);
                ConsumeModifiers(element, element.Modifiers, initialSpan);
                ProcessToken(Tokens.T_FUNCTION, initialSpan, element);
                if (element.Signature.AliasReturn)
                {
                    ConsumeToken(Tokens.T_AMP, initialSpan, element);
                }
                VisitSignature(element.Signature, element);
                var useSpan = SpanUtils.SpanIntermission(element.Signature.Span,
                    element.ReturnType != null ? element.ReturnType.Span.StartOrInvalid : (element.Body != null ? element.Body.Span.StartOrInvalid : element.Span.End));
                if (element.UseParams != null && element.UseParams.Count != 0)
                {
                    ProcessToken(Tokens.T_USE, useSpan, element);
                    ProcessToken(Tokens.T_LPAREN, useSpan, element);
                    VisitElementList(element.UseParams, Tokens.T_COMMA);
                    ProcessToken(Tokens.T_RPAREN, element.UseParams.Count > 0 ?
                        SpanUtils.SpanIntermission(element.UseParams.Last().Span, useSpan.End) : useSpan, element);
                }
                if (element.ReturnType != null)
                {
                    ProcessToken(Tokens.T_COLON, SpanUtils.SpanIntermission(element.Signature.Span,
                        element.Body != null ? element.Body.Span.StartOrInvalid : element.Span.End), element);
                    VisitElement(element.ReturnType);
                }
                if (element.Body != null)
                {
                    VisitElement(element.Body);
                }
                else
                {
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(element.Span.End - 1, 1), element);
                }
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
                        list[i] != null ? list[i].Span : Span.Invalid), list[i - 1]);
                VisitElement(list[i]);
            }
        }

        protected virtual void VisitElementList<TElement>(IList<TElement> list) where TElement : LangElement
        {
            Debug.Assert(list != null, nameof(list));
            for (int i = 0; i < list.Count; i++)
            {
                VisitElement(list[i]);
            }
        }

        protected virtual void VisitElementList(IList<UseBase> list, Tokens separatorToken, QualifiedName prefix, bool printKind, LangElement sourceNode)
        {
            // TODO - unify UseBase
            Debug.Assert(list != null, nameof(list));

            var separatorTokenText = TokenFacts.GetTokenText(separatorToken);
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ProcessToken(separatorToken, separatorTokenText,
                    SpanUtils.SpanIntermission(
                        list[i - 1] != null ? list[i - 1].Span : Span.Invalid,
                        list[i] != null ? list[i].Span : Span.Invalid), sourceNode);
                VisitUse(list[i], prefix, printKind, sourceNode);
            }
        }

        protected virtual void VisitElementList(IList<INamedTypeRef> list, Tokens separatorToken, LangElement sourceNode)
        {
            // TODO - unify INamedTypeRef
            Debug.Assert(list != null, nameof(list));

            var separatorTokenText = TokenFacts.GetTokenText(separatorToken);
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ProcessToken(separatorToken, separatorTokenText,
                    SpanUtils.SpanIntermission(
                        list[i - 1] != null ? list[i - 1].Span : Span.Invalid,
                        list[i] != null ? list[i].Span : Span.Invalid), sourceNode);
                VisitNamedTypeRef(list[i]);
            }
        }

        private void VisitNamedTypeRef(INamedTypeRef tref) => VisitElement((TypeRef)tref);

        public override void VisitLongIntLiteral(LongIntLiteral x)
        {
            ConsumeLiteral(x);
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
            ConsumeToken(Tokens.T_NAMESPACE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 9), x);

            if (x.QualifiedName.HasValue)
            {
                var qname = x.QualifiedName.QualifiedName.WithFullyQualified(false);
                VisitQualifiedName(qname, x.QualifiedName.Span, x, true);
            }

            if (x.IsSimpleSyntax)
            {
                // namespace QNAME; BODY
                ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
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
            ConsumeToken(Tokens.T_NEW, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3), x);
            if (x.ClassNameRef is AnonymousTypeRef)
            {
                VisitTypeDecl(((AnonymousTypeRef)x.ClassNameRef).TypeDeclaration, x.CallSignature);
            }
            else
            {
                VisitElement(x.ClassNameRef);
                if (x.CallSignature.Parameters.Length != 0 || x.CallSignature.Position.IsValid)
                {
                    VisitCallSignature(x.CallSignature, x);
                }
            }
        }

        public override void VisitNullableTypeRef(NullableTypeRef x)
        {
            ConsumeToken(Tokens.T_QUESTION, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 1), x);
            VisitElement(x.TargetType);
        }

        public override void VisitNullLiteral(NullLiteral x)
        {
            ConsumeLiteral(x);
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
            VisitQualifiedName(x.QualifiedName.Value, x.Span, x);
        }

        public override void VisitPseudoClassConstUse(PseudoClassConstUse x)
        {
            VisitElement(x.TargetType);
            ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TargetType.Span, x.Span.End), x);
            switch (x.Type)
            {
                case PseudoClassConstUse.Types.Class:
                    ConsumeToken(Tokens.T_CLASS, x.NamePosition, x);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override void VisitPseudoConstUse(PseudoConstUse x)
        {
            ConsumeToken(TokenFacts.GetPseudoConstUseToken(x.Type), x.Span, x);
        }

        public override void VisitRefAssignEx(RefAssignEx x)
        {
            // L =& R
            VisitElement(x.LValue);
            ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.LValue.Span, x.RValue.Span), x);
            ProcessToken(Tokens.T_AMP, SpanUtils.SpanIntermission(x.LValue.Span, x.RValue.Span), x);
            VisitElement(x.RValue);
        }

        public sealed override void VisitRefItem(RefItem x)
        {
            throw new InvalidOperationException(); // VisitArrayItem
        }

        public override void VisitReservedTypeRef(ReservedTypeRef x)
        {
            VisitQualifiedName(x.QualifiedName.Value, x.Span, x);
        }

        public override void VisitShellEx(ShellEx x)
        {
            VisitElement(x.Command);
        }

        public override void VisitStaticStmt(StaticStmt x)
        {
            ConsumeToken(Tokens.T_STATIC, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6), x);
            VisitElementList(x.StVarList, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitStaticVarDecl(StaticVarDecl x)
        {
            VisitVariableName(x.Variable, x.NameSpan, true, false, x);

            if (x.Initializer != null)
            {
                ProcessToken(Tokens.T_EQ, SpanUtils.SpanIntermission(x.NameSpan, x.Initializer.Span), x);
                VisitElement(x.Initializer);
            }
        }

        public override void VisitStringLiteral(StringLiteral x)
        {
            ConsumeLiteral(x);
        }

        public override void VisitStringLiteralDereferenceEx(StringLiteralDereferenceEx x)
        {
            VisitElement(x.StringExpr);
            ProcessToken(Tokens.T_LBRACKET, SpanUtils.SpanIntermission(x.StringExpr.Span, x.KeyExpr.Span), x);
            VisitElement(x.KeyExpr);
            ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public sealed override void VisitSwitchItem(SwitchItem x)
        {
            VisitList(x.Statements);
        }

        public override void VisitCaseItem(CaseItem x)
        {
            ConsumeToken(Tokens.T_CASE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 4), x);
            VisitElement(x.CaseVal);
            var colonSpan = SpanUtils.SpanIntermission(x.CaseVal.Span, x.Statements == null || x.Statements.Length == 0 ?
                x.Span.End : x.Statements[0].Span.StartOrInvalid);
            ProcessToken(Tokens.T_COLON, colonSpan, x);
            VisitSwitchItem(x);
        }

        public override void VisitDefaultItem(DefaultItem x)
        {
            var labelSpan = SpanUtils.SafeSpan(x.Span.StartOrInvalid, 7);
            ConsumeToken(Tokens.T_DEFAULT, labelSpan, x);
            var colonSpan = SpanUtils.SpanIntermission(labelSpan, x.Statements == null || x.Statements.Length == 0 ?
                x.Span.End : x.Statements[0].Span.StartOrInvalid);
            ProcessToken(Tokens.T_COLON, colonSpan, x);
            VisitSwitchItem(x);
        }

        public override void VisitSwitchStmt(SwitchStmt x)
        {
            // switch(VALUE){CASES}
            ConsumeToken(Tokens.T_SWITCH, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 6), x);
            ProcessToken(Tokens.T_LPAREN, x.Span, x);
            VisitElement(x.SwitchValue);
            var braceSpan = SpanUtils.SpanIntermission(x.SwitchValue.Span,
                x.SwitchItems.Length > 0 ? x.SwitchItems.First().Span.StartOrInvalid : x.Span.End);
            ProcessToken(Tokens.T_RPAREN, braceSpan, x);
            var separator = _provider.GetTokenAt(braceSpan, Tokens.T_COLON, null) ??
                _provider.GetTokenAt(braceSpan, Tokens.T_LBRACE,
                SwitchShortNotation ? new SourceToken(Tokens.T_LBRACE, Span.Invalid) : new SourceToken(Tokens.T_COLON, Span.Invalid));
            ConsumeToken(separator, x);
            var token = _provider.GetTokenAt(braceSpan, Tokens.T_SEMI, null);
            if (token != null)
            {
                ConsumeToken(token, x);
            }
            VisitList(x.SwitchItems);
            var endSpan = SpanUtils.SpanIntermission(x.SwitchItems.Length > 0 ? x.SwitchItems.Last().Span : x.SwitchValue.Span, x.Span.End);
            var tokens = _provider.GetTokens(endSpan, t => t.Token == Tokens.T_ENDSWITCH || t.Token == Tokens.T_SEMI, null).CastToArray<ISourceToken>();
            if (tokens != null && tokens.Length == 2 && tokens[0].Token == Tokens.T_ENDSWITCH && tokens[1].Token == Tokens.T_SEMI)
            {
                ConsumeToken(tokens[0], x);
                ConsumeToken(tokens[1], x);
            }
            else
            {
                if (separator.Token == Tokens.T_COLON)
                {
                    ConsumeToken(Tokens.T_ENDSWITCH, SpanUtils.SafeSpan(x.Span.End - 10, 9), x);
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
                }
                else
                {
                    ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
                }
            }
        }

        public override void VisitThrowStmt(ThrowStmt x)
        {
            // throw EXPR;
            ConsumeToken(Tokens.T_THROW, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitTraitAdaptationAlias(TraitsUse.TraitAdaptationAlias x)
        {
            if (x.TraitMemberName.Item1 != null)
            {
                VisitQualifiedName(x.TraitMemberName.Item1.QualifiedName.Value, x.TraitMemberName.Item1.Span, x);
                ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TraitMemberName.Item1.Span, x.TraitMemberName.Item2.Span), x);
            }
            ConsumeToken(Tokens.T_STRING, x.TraitMemberName.Item2.Name.Value, x.TraitMemberName.Item2.Span, x);
            var asSpan = SpanUtils.SpanIntermission(x.TraitMemberName.Item2.Span,
                x.NewName.HasValue ? x.NewName.Span.StartOrInvalid : x.Span.End);
            var token = ProcessToken(Tokens.T_AS, asSpan, x);
            if (x.NewModifier.HasValue)
            {
                ConsumeModifiers(x, x.NewModifier.Value, SpanUtils.SpanIntermission(token.Span,
                    x.NewName.HasValue ? x.NewName.Span.StartOrInvalid : x.Span.End));
            }
            if (x.NewName.HasValue)
            {
                ConsumeNameToken(x.NewName.Name.Value, x.NewName.Span, x);
            }
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        private void ConsumeNameToken(string value, Span span, LangElement sourceElement)
        {
            if (TokenFacts.s_reservedNameToToken.TryGetValue(value, out Tokens t) == false)
            {
                t = Tokens.T_STRING;
            }

            ConsumeToken(t, value, span, sourceElement);
        }

        public override void VisitTraitAdaptationPrecedence(TraitsUse.TraitAdaptationPrecedence x)
        {
            if (x.TraitMemberName.Item1 != null)
            {
                VisitQualifiedName(x.TraitMemberName.Item1.QualifiedName.Value, x.TraitMemberName.Item1.Span, x);
                ProcessToken(Tokens.T_DOUBLE_COLON, SpanUtils.SpanIntermission(x.TraitMemberName.Item1.Span, x.TraitMemberName.Item2.Span), x);
            }
            ConsumeNameToken(x.TraitMemberName.Item2.Name.Value, x.TraitMemberName.Item2.Span, x);
            ProcessToken(Tokens.T_INSTEADOF, SpanUtils.SpanIntermission(x.TraitMemberName.Item2.Span,
                x.IgnoredTypes != null && x.IgnoredTypes.Length != 0 ? x.IgnoredTypes[0].Span.StartOrInvalid : x.Span.End), x);
            VisitElementList(x.IgnoredTypes, Tokens.T_COMMA);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitTraitAdaptationBlock(TraitAdaptationBlock x)
        {
            ConsumeToken(Tokens.T_LBRACE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 1), x);
            VisitElementList(x.Adaptations);
            ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitTraitsUse(TraitsUse x)
        {
            ConsumeToken(Tokens.T_USE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3), x);
            VisitElementList(x.TraitsList, Tokens.T_COMMA);
            if (x.TraitAdaptationBlock != null)
            {
                VisitElement(x.TraitAdaptationBlock);
            }
            else
            {
                ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
            }
        }

        public override void VisitTranslatedTypeRef(TranslatedTypeRef x)
        {
            VisitElement(x.OriginalType);
        }

        public override void VisitTryStmt(TryStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_TRY, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3), x);
                VisitElement(x.Body);
                VisitList(x.Catches);
                VisitElement(x.FinallyItem);
            }
        }

        public override void VisitTypeDecl(TypeDecl x)
        {
            VisitTypeDecl(x, null);
        }

        private void VisitTypeDecl(TypeDecl x, CallSignature signature)
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
                    previous = ProcessToken(Tokens.T_CLASS, prenameSpan, x);
                    if (signature != null && signature.Parameters.Length != 0)
                    {
                        VisitCallSignature(signature, x);
                    }
                }
                if (x.Name.HasValue && !x.Name.Name.IsGenerated)
                {
                    previous = ProcessToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span, x);
                }
                if (x.BaseClass != null)
                {
                    // extends
                    previous = ProcessToken(Tokens.T_EXTENDS, SpanUtils.SpanIntermission(previous.Span, baseSpan), x);
                    VisitElement((TypeRef)x.BaseClass);
                }
                if (x.ImplementsList != null && x.ImplementsList.Length != 0)
                {
                    // implements|extends
                    if ((x.MemberAttributes & PhpMemberAttributes.Interface) == 0)
                    {
                        previous = ProcessToken(Tokens.T_IMPLEMENTS, SpanUtils.SpanIntermission(previous.Span, implementsSpan), x);
                    }
                    else
                    {
                        previous = ProcessToken(Tokens.T_EXTENDS, SpanUtils.SpanIntermission(previous.Span, implementsSpan), x);
                    }

                    VisitElementList(x.ImplementsList, Tokens.T_COMMA, x);
                }

                ProcessToken(Tokens.T_LBRACE, SpanUtils.SpanIntermission(previous.Span, bodySpan), x);
                VisitList(x.Members);
                ConsumeToken(Tokens.T_RBRACE, lastSpan, x);
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
            ConsumeToken(token, text, SpanUtils.SafeSpan(x.Span.StartOrInvalid, text.Length), x);
            VisitElement(x.Expr);
        }

        public override void VisitUnsetStmt(UnsetStmt x)
        {
            ConsumeToken(Tokens.T_UNSET, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);
            ProcessToken(Tokens.T_LPAREN, x.Span, x);
            VisitElementList(x.VarList, Tokens.T_COMMA);
            ProcessToken(Tokens.T_RPAREN, x.VarList.Count > 0 ? SpanUtils.SpanIntermission(x.VarList.Last().Span, x.Span.End) : x.Span, x);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        public override void VisitUseStatement(UseStatement x)
        {
            ConsumeToken(Tokens.T_USE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 3), x);
            switch (x.Kind)
            {
                case AliasKind.Constant: ProcessToken(Tokens.T_CONST, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x); break;
                case AliasKind.Function: ProcessToken(Tokens.T_FUNCTION, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 8), x); break;
            }

            VisitElementList(x.Uses, Tokens.T_COMMA, new QualifiedName(), x.Kind == AliasKind.Type && x.Uses.Length == 1 && x.Uses[0] is GroupUse, x);
            ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
        }

        protected virtual void VisitUse(SimpleUse use, QualifiedName prefix, bool printKind, LangElement sourceNode)
        {
            if (printKind)
            {
                switch (use.Alias.Kind)
                {
                    case AliasKind.Constant: ConsumeToken(Tokens.T_CONST, SpanUtils.SafeSpan(use.Span.StartOrInvalid, 5), sourceNode); break;
                    case AliasKind.Function: ConsumeToken(Tokens.T_FUNCTION, SpanUtils.SafeSpan(use.Span.StartOrInvalid, 8), sourceNode); break;
                }
            }
            var offset = prefix.Namespaces == null ? 0 : prefix.Namespaces.Length;
            Name[] namespaces = new Name[use.QualifiedName.Namespaces.Length - offset];
            for (int i = 0; i < namespaces.Length; i++)
            {
                namespaces[i] = use.QualifiedName.Namespaces[i + offset];
            }
            VisitQualifiedName(new QualifiedName(use.QualifiedName.Name, namespaces), use.NameSpan, sourceNode);
            if (use.QualifiedName.Name.Value != use.Alias.Name.Value)
            {
                ProcessToken(Tokens.T_AS, SpanUtils.SpanIntermission(use.NameSpan, use.AliasSpan), sourceNode);
                ConsumeNameToken(use.Alias.Name.Value, use.AliasSpan, sourceNode);
            }
        }

        protected virtual void VisitUse(GroupUse use, bool printKind, LangElement sourceNode)
        {
            VisitQualifiedName(use.Prefix.QualifiedName, use.Prefix.Span, sourceNode);
            var span = SpanUtils.SpanIntermission(use.Prefix.Span, use.Span.End);
            ProcessToken(Tokens.T_LBRACE, span, sourceNode);
            VisitElementList(use.Uses, Tokens.T_COMMA, use.Prefix.QualifiedName, printKind, sourceNode);
            ConsumeToken(Tokens.T_RBRACE, SpanUtils.SafeSpan(use.Span.End - 1, 1), sourceNode);
        }

        protected virtual void VisitUse(UseBase use, QualifiedName prefix, bool printKind, LangElement sourceNode)
        {
            if (use is SimpleUse)
            {
                VisitUse((SimpleUse)use, prefix, printKind, sourceNode);
            }
            else if (use is GroupUse)
            {
                VisitUse((GroupUse)use, printKind, sourceNode);
            }
        }

        public override void VisitValueAssignEx(ValueAssignEx x)
        {
            // L = R
            VisitElement(x.LValue);
            ProcessToken(TokenFacts.GetOperationToken(x.Operation), SpanUtils.SpanIntermission(x.LValue.Span, x.RValue.Span), x);
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
                    ConsumeToken(Tokens.T_WHILE, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);
                    ProcessToken(Tokens.T_LPAREN, SpanUtils.SpanIntermission(x.Span.Start, x.CondExpr.Span), x);
                    VisitElement(x.CondExpr);
                    ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.CondExpr.Span, x.Body.Span), x);
                    VisitElement(x.Body);
                }
                else
                {
                    ConsumeToken(Tokens.T_DO, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 2), x);
                    VisitElement(x.Body);
                    Span whileSpan = SpanUtils.SpanIntermission(x.Body.Span, x.CondExpr.Span);
                    ProcessToken(Tokens.T_WHILE, whileSpan, x);
                    ProcessToken(Tokens.T_LPAREN, whileSpan, x);
                    VisitElement(x.CondExpr);
                    ProcessToken(Tokens.T_RPAREN, SpanUtils.SpanIntermission(x.CondExpr.Span, x.Span.End), x);
                    ConsumeToken(Tokens.T_SEMI, SpanUtils.SafeSpan(x.Span.End - 1, 1), x);
                }
            }
        }

        public override void VisitYieldEx(YieldEx x)
        {
            ConsumeToken(Tokens.T_YIELD, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 5), x);

            if (x.KeyExpr != null)
            {
                VisitElement(x.KeyExpr);
                ProcessToken(Tokens.T_DOUBLE_ARROW, SpanUtils.SpanIntermission(x.KeyExpr.Span, x.ValueExpr.Span), x);
            }

            VisitElement(x.ValueExpr);
        }

        public override void VisitYieldFromEx(YieldFromEx x)
        {
            ConsumeToken(Tokens.T_YIELD_FROM, SpanUtils.SafeSpan(x.Span.StartOrInvalid, 10), x);
            VisitElement(x.ValueExpr);
        }

        #endregion
    }
}
