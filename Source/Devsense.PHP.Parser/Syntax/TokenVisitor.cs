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
        /// Whether to tokenize according to the old syntax <code>array(...)</code> or the new syntax <code>[...]</code>
        /// </summary>
        bool IsOldArraySyntax(ArrayEx node);

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
        /// <param name="originalSpan">Original position of the AST node in source code.</param>
        void ConsumeToken(Tokens token, string text, int position, Span originalSpan);
    }

    #region DefaultTokenVisitorOptions

    /// <summary>
    /// A default options implementation with default values.
    /// </summary>
    public class DefaultTokenComposer : ITokenComposer
    {
        public static ITokenComposer Instance = new DefaultTokenComposer();

        protected DefaultTokenComposer() { }

        /// <summary>
        /// Shortcut for <see cref="ConsumeToken(Tokens, string, int)"/>.
        /// </summary>
        protected void ConsumeToken(Tokens token, int position, Span originalSpan) => ConsumeToken(token, TokenFacts.GetTokenText(token), position, originalSpan);

        /// <inheritdoc />
        public virtual void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            if ((modifiers & PhpMemberAttributes.Private) != 0) ConsumeToken(Tokens.T_PRIVATE, span.StartOrInvalid, span);
            if ((modifiers & PhpMemberAttributes.Protected) != 0) ConsumeToken(Tokens.T_PROTECTED, span.StartOrInvalid, span);

            if ((modifiers & PhpMemberAttributes.Static) != 0) ConsumeToken(Tokens.T_STATIC, span.StartOrInvalid, span);
            if ((modifiers & PhpMemberAttributes.Abstract) != 0) ConsumeToken(Tokens.T_ABSTRACT, span.StartOrInvalid, span);
            if ((modifiers & PhpMemberAttributes.Final) != 0) ConsumeToken(Tokens.T_FINAL, span.StartOrInvalid, span);
        }

        /// <inheritdoc />
        public virtual void ConsumeToken(Tokens token, string text, int position, Span originalSpan)
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
                ConsumeToken(Tokens.T_STRING, /*IsUpperCase(x) ? value.ToUpperInvariant() :*/ value.ToLowerInvariant(), literal.Span.StartOrInvalid, literal.Span);
            }
            else if (literal is DoubleLiteral)
            {
                ConsumeToken(Tokens.T_DNUMBER, ((DoubleLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span.StartOrInvalid, literal.Span);
            }
            else if (literal is NullLiteral)
            {
                ConsumeToken(Tokens.T_STRING, /*_composer.IsUpperCase(x) ? "NULL" :*/ "null", literal.Span.StartOrInvalid, literal.Span);
            }
            else if (literal is LongIntLiteral)
            {
                ConsumeToken(Tokens.T_LNUMBER, ((LongIntLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span.StartOrInvalid, literal.Span);
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

        public TokenVisitor(TreeContext initialContext, ITokenComposer composer) : base(initialContext)
        {
            _composer = composer ?? DefaultTokenComposer.Instance;  // TODO: to TreeContext
        }

        /// <summary>
        /// Invoked when a token is visited.
        /// </summary>
        /// <param name="token">Token id.</param>
        /// <param name="text">Textual representation of <paramref name="token"/>.</param>
        /// <param name="position">Optional. Original position in source code.</param>
        public void ConsumeToken(Tokens token, string text, int position, Span originalSpan)
        {
            _composer.ConsumeToken(token, text, position, originalSpan);
        }

        protected void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            _composer.ConsumeModifiers(element, modifiers, span);
        }

        /// <summary>
        /// Shortcut for <see cref="ConsumeToken(Tokens, string, int, Span)"/>.
        /// </summary>
        protected void ConsumeToken(Tokens token, int position, Span originalSpan) => ConsumeToken(token, TokenFacts.GetTokenText(token), position, originalSpan);

        #region Single Nodes Overrides

        public override void VisitElement(LangElement element)
        {
            base.VisitElement(element);
        }

        public override void VisitActualParam(ActualParam x)
        {
            if (x.IsUnpack) ConsumeToken(Tokens.T_ELLIPSIS, "...", x.Span.StartOrInvalid, x.Span);
            if (x.Ampersand) ConsumeToken(Tokens.T_AMP, "&", x.Span.StartOrInvalid, x.Span);
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
            if (_composer.IsOldArraySyntax(x))
            {
                ConsumeToken(Tokens.T_ARRAY, "array", x.Span.StartOrInvalid, x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 5, x.Span);
                VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
            }
            else
            {
                ConsumeToken(Tokens.T_LBRACKET, "[", x.Span.StartOrInvalid, x.Span);
                VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_RBRACKET, "]", x.Span.End - 1, x.Span);
            }
        }

        public override void VisitArrayItem(Item item)
        {
            if (item != null)
            {
                if (item.Index != null)
                {
                    VisitElement(item.Index);
                    ConsumeToken(Tokens.T_DOUBLE_ARROW, "=>", item.Index.Span.End, item.Index.Span);
                }

                if (item is ValueItem)
                {
                    VisitElement(((ValueItem)item).ValueExpr);
                }
                else if (item is RefItem)
                {
                    ConsumeToken(Tokens.T_AMP, "&", item.Index.Span.StartOrInvalid, item.Index.Span);
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
            ConsumeToken(Tokens.T_STRING, "assert", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 6, x.Span);
            VisitElement(x.CodeEx);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
        }

        public sealed override void VisitAssignEx(AssignEx x) { throw new InvalidOperationException(); }

        public override void VisitBinaryEx(BinaryEx x)
        {
            VisitElement(x.LeftExpr);
            ConsumeToken(TokenFacts.GetOperationToken(x.Operation), x.OperationPosition, x.Span);
            VisitElement(x.RightExpr);
        }

        public override void VisitBinaryStringLiteral(BinaryStringLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitBlockStmt(BlockStmt x)
        {
            ConsumeToken(Tokens.T_LBRACE, "{", x.Span.StartOrInvalid, x.Span);
            base.VisitBlockStmt(x);
            ConsumeToken(Tokens.T_RBRACE, "}", x.Span.End - 1, x.Span);
        }

        public override void VisitBoolLiteral(BoolLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitCaseItem(CaseItem x)
        {
            ConsumeToken(Tokens.T_CASE, "case", x.Span.StartOrInvalid, x.Span);
            VisitElement(x.CaseVal);
            ConsumeToken(Tokens.T_COLON, ":", x.Span.End - 1, x.Span);

            base.VisitCaseItem(x);
        }

        public override void VisitCatchItem(CatchItem x)
        {
            // catch (TYPE VARIABLE) BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_CATCH, "catch", x.Span.StartOrInvalid, x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 5, x.Span);
                VisitElement(x.TargetType);
                VisitElement(x.Variable);
                ConsumeToken(Tokens.T_RPAREN, ")", x.Body.Span.StartOrInvalid - 1, x.Span);
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
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", x.NamePosition.StartOrInvalid - 2, x.Span);
            ConsumeToken(Tokens.T_STRING, x.Name.Value, x.NamePosition.StartOrInvalid, x.Span);
        }

        public override void VisitClassTypeRef(ClassTypeRef x)
        {
            VisitQualifiedName(x.ClassName, x.Span.StartOrInvalid, x.Span);
        }

        public override void VisitConcatEx(ConcatEx x)
        {
            VisitElementList(x.Expressions, Tokens.T_DOT, ".");
        }

        public override void VisitConditionalEx(ConditionalEx x)
        {
            VisitElement(x.CondExpr);
            ConsumeToken(Tokens.T_QUESTION, "?", x.CondExpr.Span.End, x.Span);
            VisitElement(x.TrueExpr);   // can be null
            ConsumeToken(Tokens.T_COLON, ":", x.TrueExpr.Span.End, x.Span);
            VisitElement(x.FalseExpr);
        }

        public sealed override void VisitConditionalStmt(ConditionalStmt x)
        {
            throw new InvalidOperationException();  // VisitIfStmt
        }

        public override void VisitConstantDecl(ConstantDecl x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span.StartOrInvalid, x.Span);

            if (x.Initializer != null)  // always true
            {
                ConsumeToken(Tokens.T_EQ, "=", x.Name.Span.End, x.Span);
                VisitElement(x.Initializer);
            }
        }

        public sealed override void VisitConstantUse(ConstantUse x)
        {
            throw new InvalidOperationException(); // override PseudoConstant, ClassCOnstant, GlobalConstant
        }

        public override void VisitConstDeclList(ConstDeclList x)
        {
            ConsumeModifiers(x, x.Modifiers, x.Span.IsValid && x.Constants[0].Span.IsValid ? Span.FromBounds(x.Span.StartOrInvalid, x.Constants[0].Span.StartOrInvalid) : Span.Invalid);
            ConsumeToken(Tokens.T_CONST, "const", x.Span.StartOrInvalid, x.Span);
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
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
            ConsumeToken(Tokens.T_DEFAULT, "default", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_COLON, ":", x.Span.StartOrInvalid + 7, x.Span);
            VisitList(x.Statements);
        }

        public override void VisitDirectFcnCall(DirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitQualifiedName(x.FullName.OriginalName, x.Span.StartOrInvalid, x.Span);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectStFldUse(DirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", x.NameSpan.StartOrInvalid - 2, x.Span);
            VisitVariableName(x.PropertyName, x.NameSpan, true);  // $name
        }

        public override void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", x.MethodName.Span.StartOrInvalid - 2, x.Span);
            ConsumeToken(Tokens.T_STRING, x.MethodName.Name.Value, x.MethodName.Span.StartOrInvalid, x.Span);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectVarUse(DirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitVariableName(x.VarName, x.Span, x.IsMemberOf == null);
        }

        public override void VisitDoubleLiteral(DoubleLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitEchoStmt(EchoStmt x)
        {
            if (x.IsHtmlCode)
            {
                ConsumeToken(Tokens.T_INLINE_HTML, ((StringLiteral)x.Parameters[0]).Value, x.Span.StartOrInvalid, x.Span);
            }
            else
            {
                // echo PARAMETERS;
                ConsumeToken(Tokens.T_ECHO, "echo", x.Span.StartOrInvalid, x.Span);
                VisitElementList(x.Parameters, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
            }
        }

        public override void VisitParenthesisExpression(ParenthesisExpression x)
        {
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid, x.Span);
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
        }

        public override void VisitEmptyEx(EmptyEx x)
        {
            // empty(OPERAND)
            ConsumeToken(Tokens.T_EMPTY, "empty", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 5, x.Span);
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
        }

        public override void VisitEmptyStmt(EmptyStmt x)
        {
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.StartOrInvalid, x.Span);
        }

        public override void VisitEvalEx(EvalEx x)
        {
            ConsumeToken(Tokens.T_EVAL, "eval", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 4, x.Span);
            VisitElement(x.Code);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
        }

        public override void VisitExitEx(ExitEx x)
        {
            ConsumeToken(Tokens.T_EXIT, "exit", x.Span.StartOrInvalid, x.Span);
            VisitElement(x.ResulExpr);
        }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            base.VisitExpressionStmt(x);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitFieldDecl(FieldDecl x)
        {
            VisitVariableName(x.Name, x.NameSpan, true);
            if (x.Initializer != null)
            {
                ConsumeToken(Tokens.T_EQ, "=", x.Initializer.Span.StartOrInvalid - 1, x.Span);
                VisitElement(x.Initializer);
            }
        }

        public override void VisitFieldDeclList(FieldDeclList x)
        {
            ConsumeModifiers(x, x.Modifiers, x.Span.IsValid && x.Fields[0].Span.IsValid ? Span.FromBounds(x.Span.StartOrInvalid, x.Fields[0].Span.StartOrInvalid) : Span.Invalid);

            if (x.Modifiers == 0)
            {
                ConsumeToken(Tokens.T_VAR, "var", x.Span.StartOrInvalid, x.Span);
            }

            VisitElementList(x.Fields, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            // finally BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FINAL, "finally", x.Span.StartOrInvalid, x.Span);
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FOREACH, "foreach", x.Span.StartOrInvalid, x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 7, x.Span);
                VisitElement(x.Enumeree);
                ConsumeToken(Tokens.T_AS, "as", x.Enumeree.Span.End + 1, x.Span);
                if (x.KeyVariable != null)
                {
                    VisitForeachVar(x.KeyVariable);
                    ConsumeToken(Tokens.T_DOUBLE_ARROW, "=>", x.KeyVariable.Span.End + 1, x.Span);
                }
                VisitForeachVar(x.ValueVariable);
                ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachVar(ForeachVar x)
        {
            if (x.Alias) ConsumeToken(Tokens.T_AMP, "&", x.Span.StartOrInvalid, x.Span);
            VisitElement(x.Target);
        }

        public override void VisitFormalParam(FormalParam x)
        {
            VisitElement(x.TypeHint);
            if (x.PassedByRef)
            {
                ConsumeToken(Tokens.T_AMP, "&", x.Span.StartOrInvalid, x.Span);
            }
            if (x.IsVariadic)
            {
                ConsumeToken(Tokens.T_ELLIPSIS, "...", x.Span.StartOrInvalid, x.Span);
            }

            VisitVariableName(x.Name.Name, x.Name.Span, true);
            if (x.InitValue != null)
            {
                ConsumeToken(Tokens.T_EQ, "=", x.InitValue.Span.StartOrInvalid - 1, x.Span);
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
                ConsumeToken(Tokens.T_FOR, "for", x.Span.StartOrInvalid, x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 3, x.Span);

                VisitElementList(x.InitExList, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";", x.InitExList.Count > 0 ?
                    x.InitExList.Last().Span.End : x.Span.StartOrInvalid + 4, x.Span);
                VisitElementList(x.CondExList, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";", x.CondExList.Count > 0 ?
                    x.CondExList.Last().Span.End : x.Span.StartOrInvalid + 5, x.Span);
                VisitElementList(x.ActionExList, Tokens.T_COMMA, ",");

                ConsumeToken(Tokens.T_LPAREN, ")", x.Span.End - 1, x.Span);

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
                ConsumeModifiers(element, modifiers, element.Span.IsValid && nameOpt.Span.IsValid ? Span.FromBounds(element.Span.StartOrInvalid, nameOpt.Span.StartOrInvalid) : Span.Invalid);
                ConsumeToken(Tokens.T_FUNCTION, "function", element.Span.StartOrInvalid, element.Span);
                if (signature.AliasReturn) ConsumeToken(Tokens.T_AMP, "&", element.Span.StartOrInvalid + 8, element.Span);
                if (nameOpt.HasValue) ConsumeToken(Tokens.T_STRING, nameOpt.Name.Value, nameOpt.Span.StartOrInvalid, element.Span);
                VisitSignature(signature);
                if (returnTypeOpt != null)
                {
                    ConsumeToken(Tokens.T_COLON, ":", returnTypeOpt.Span.StartOrInvalid - 1, element.Span);
                    VisitElement(returnTypeOpt);
                }
                VisitElement(body);
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
            ConsumeToken(Tokens.T_CONST, "const", x.Span.StartOrInvalid, x.Span);
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitGlobalConstUse(GlobalConstUse x)
        {
            VisitQualifiedName(x.FullName.OriginalName, x.Span.StartOrInvalid, x.Span);
        }

        public override void VisitGlobalStmt(GlobalStmt x)
        {
            ConsumeToken(Tokens.T_GLOBAL, x.Span.StartOrInvalid, x.Span);
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitGotoStmt(GotoStmt x)
        {
            ConsumeToken(Tokens.T_GOTO, "goto", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_STRING, x.LabelName.Name.Value, x.LabelName.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitHaltCompiler(HaltCompiler x)
        {
            ConsumeToken(Tokens.T_HALT_COMPILER, "__halt_compiler", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.End - 3, x.Span);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 2, x.Span);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
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
                        ConsumeToken(Tokens.T_IF, cond.Span.StartOrInvalid, x.Span);
                    }
                    else if (cond.Condition != null)
                    {
                        ConsumeToken(Tokens.T_ELSEIF, cond.Span.StartOrInvalid, x.Span);
                    }
                    else
                    {
                        ConsumeToken(Tokens.T_ELSE, cond.Span.StartOrInvalid, x.Span);
                    }

                    if (cond.Condition != null)
                    {
                        ConsumeToken(Tokens.T_LPAREN, "(", cond.Condition.Span.StartOrInvalid - 1, x.Span);
                        VisitElement(cond.Condition);
                        ConsumeToken(Tokens.T_RPAREN, ")", cond.Condition.Span.End, x.Span);
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
            ConsumeToken(x.Inc ? Tokens.T_INC : Tokens.T_DEC, x.Inc ? "++" : "--", x.Post ? x.Span.End - 2 : x.Span.StartOrInvalid, x.Span);

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
                    ConsumeToken(Tokens.T_INCLUDE, "include", x.Span.StartOrInvalid, x.Span);
                    break;
                case InclusionTypes.IncludeOnce:
                    ConsumeToken(Tokens.T_INCLUDE_ONCE, "include_once", x.Span.StartOrInvalid, x.Span);
                    break;
                case InclusionTypes.Require:
                    ConsumeToken(Tokens.T_REQUIRE, "require", x.Span.StartOrInvalid, x.Span);
                    break;
                case InclusionTypes.RequireOnce:
                    ConsumeToken(Tokens.T_REQUIRE_ONCE, "require_once", x.Span.StartOrInvalid, x.Span);
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
            ConsumeToken(Tokens.T_VARIABLE, (dollar ? "$" : string.Empty) + name.Value, span.StartOrInvalid, span);
        }

        public virtual void VisitQualifiedName(QualifiedName qname, int position, Span originalSpan)
        {
            if (qname.IsFullyQualifiedName)
            {
                ConsumeToken(Tokens.T_NS_SEPARATOR, QualifiedName.Separator.ToString(), position, originalSpan);
            }

            var ns = qname.Namespaces;
            for (int i = 0; i < ns.Length; i++)
            {
                ConsumeToken(Tokens.T_STRING, ns[i].Value, position, originalSpan);
                ConsumeToken(Tokens.T_NS_SEPARATOR, QualifiedName.Separator.ToString(), position, originalSpan);
            }

            ConsumeToken(Tokens.T_STRING, qname.Name.Value, position, originalSpan);
        }

        public virtual void VisitCallSignature(CallSignature signature)
        {
            var first = signature.Parameters.Length > 0 ? signature.Parameters[0] : null;
            var last = signature.Parameters.Length > 0 ? signature.Parameters[signature.Parameters.Length - 1] : null;
            var span = signature.Parameters.Length > 0 ? Span.FromBounds(first.Span.Start - 1, last.Span.End + 1) : Span.Invalid;
            ConsumeToken(Tokens.T_LPAREN, "(", span.StartOrInvalid, span);
            VisitElementList(signature.Parameters, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", span.End - 1, span);
        }

        public virtual void VisitSignature(Signature signature)
        {
            var first = signature.FormalParams.Length > 0 ? signature.FormalParams[0] : null;
            var last = signature.FormalParams.Length > 0 ? signature.FormalParams[signature.FormalParams.Length - 1] : null;
            var span = signature.FormalParams.Length > 0 ? Span.FromBounds(first.Span.Start - 1, last.Span.End + 1) : Span.Invalid;
            ConsumeToken(Tokens.T_LPAREN, "(", span.StartOrInvalid, span);
            VisitElementList(signature.FormalParams, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", span.End - 1, span);
        }

        public override void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", x.TargetType.Span.End, x.Span);
            VisitElement(x.FieldNameExpr);  // TODO: { ... } ?
        }

        public override void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", x.TargetType.Span.End, x.Span);
            VisitElement(x.MethodNameExpression);  // TODO: { ... } ?
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitIndirectTypeRef(IndirectTypeRef x)
        {
            VisitElement(x.ClassNameVar);
        }

        public override void VisitIndirectVarUse(IndirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitElement(x.VarNameEx);    // TODO: { ... } ?
        }

        public override void VisitInstanceOfEx(InstanceOfEx x)
        {
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_INSTANCEOF, "instanceof", x.Expression.Span.End, x.Span);
            VisitElement(x.ClassNameRef);
        }

        public override void VisitIssetEx(IssetEx x)
        {
            ConsumeToken(Tokens.T_ISSET, "isset", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 5, x.Span);
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
        }

        public override void VisitItemUse(ItemUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitElement(x.Array);
            ConsumeToken(Tokens.T_LBRACKET, "[", x.Index.Span.StartOrInvalid - 1, x.Span);
            VisitElement(x.Index);
            ConsumeToken(Tokens.T_RBRACKET, "]", x.Span.End - 1, x.Span);
        }

        public virtual void VisitIsMemberOf(Expression isMemberOf)
        {
            if (isMemberOf != null)
            {
                VisitElement(isMemberOf);
                ConsumeToken(Tokens.T_OBJECT_OPERATOR, "->", isMemberOf.Span.End, isMemberOf.Span);
            }
        }

        public override void VisitJumpStmt(JumpStmt x)
        {
            switch (x.Type)
            {
                case JumpStmt.Types.Return:
                    ConsumeToken(Tokens.T_RETURN, "return", x.Span.StartOrInvalid, x.Span);
                    break;
                case JumpStmt.Types.Continue:
                    ConsumeToken(Tokens.T_CONTINUE, "continue", x.Span.StartOrInvalid, x.Span);
                    break;
                case JumpStmt.Types.Break:
                    ConsumeToken(Tokens.T_BREAK, "break", x.Span.StartOrInvalid, x.Span);
                    break;
            }

            VisitElement(x.Expression);

            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitLabelStmt(LabelStmt x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_COLON, ":", x.Span.End - 1, x.Span);
        }

        public override void VisitLambdaFunctionExpr(LambdaFunctionExpr x)
        {
            VisitRoutineDecl(x, x.Signature, x.Body,
                returnTypeOpt: x.ReturnType,
                modifiers: x.IsStatic ? PhpMemberAttributes.Static : PhpMemberAttributes.None);
        }

        public override void VisitListEx(ListEx x)
        {
            ConsumeToken(Tokens.T_LIST, "list", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 4, x.Span);
            VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1, x.Span);
        }

        protected virtual void VisitElementList<TElement>(IList<TElement> list, Tokens separatorToken, string separatorTokenText) where TElement : LangElement
        {
            Debug.Assert(list != null, nameof(list));

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ConsumeToken(separatorToken, separatorTokenText, list[i - 1].Span.End, list[i].Span);
                VisitElement(list[i]);
            }
        }

        protected virtual void VisitElementList<TElement>(IList<TElement> list, Action<TElement> action, Tokens separatorToken, string separatorTokenText)
        {
            Debug.Assert(list != null, nameof(list));
            Debug.Assert(action != null, nameof(action));

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ConsumeToken(separatorToken, separatorTokenText, -1, Span.Invalid);
                action(list[i]);
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
            ConsumeToken(Tokens.T_NAMESPACE, "namespace", x.Span.StartOrInvalid, x.Span);

            if (x.QualifiedName.HasValue)
            {
                VisitQualifiedName(x.QualifiedName.QualifiedName.WithFullyQualified(false), x.Span.StartOrInvalid, x.Span);
            }

            if (x.IsSimpleSyntax)
            {
                // namespace QNAME; BODY
                ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
                VisitList(x.Body.Statements);
            }
            else
            {
                // namespace QNAME { BODY }
            }

            VisitElement(x.Body);
        }

        public override void VisitNewEx(NewEx x)
        {
            ConsumeToken(Tokens.T_NEW, "new", x.Span.StartOrInvalid, x.Span);
            VisitElement(x.ClassNameRef);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitNullableTypeRef(NullableTypeRef x)
        {
            ConsumeToken(Tokens.T_QUESTION, "?", x.Span.StartOrInvalid, x.Span);
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
            VisitQualifiedName(x.QualifiedName.Value, x.Span.StartOrInvalid, x.Span);
        }

        public override void VisitPseudoClassConstUse(PseudoClassConstUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::", x.TargetType.Span.End, x.Span);
            switch (x.Type)
            {
                case PseudoClassConstUse.Types.Class:
                    ConsumeToken(Tokens.T_CLASS, "class", x.NamePosition.StartOrInvalid, x.Span);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public override void VisitPseudoConstUse(PseudoConstUse x)
        {
            ConsumeToken(TokenFacts.GetPseudoConstUseToken(x.Type), x.Span.StartOrInvalid, x.Span);
        }

        public override void VisitRefAssignEx(RefAssignEx x)
        {
            // L =& R
            VisitElement(x.LValue);
            ConsumeToken(Tokens.T_EQ, "=", x.LValue.Span.End, x.Span);
            ConsumeToken(Tokens.T_AMP, "&", x.LValue.Span.End + 1, x.Span);
            VisitElement(x.RValue);
        }

        public sealed override void VisitRefItem(RefItem x)
        {
            throw new InvalidOperationException(); // VisitArrayItem
        }

        public override void VisitReservedTypeRef(ReservedTypeRef x)
        {
            VisitQualifiedName(x.QualifiedName.Value, x.Span.StartOrInvalid, x.Span);
        }

        public override void VisitShellEx(ShellEx x)
        {
            ConsumeToken(Tokens.T_BACKQUOTE, x.Span.StartOrInvalid, x.Span);
            // content
            ConsumeToken(Tokens.T_BACKQUOTE, x.Span.End - 1, x.Span);
            throw new NotImplementedException();
        }

        public override void VisitStaticStmt(StaticStmt x)
        {
            ConsumeToken(Tokens.T_STATIC, "static", x.Span.StartOrInvalid, x.Span);
            VisitElementList(x.StVarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitStaticVarDecl(StaticVarDecl x)
        {
            VisitVariableName(x.Variable, x.NameSpan, true);

            if (x.Initializer != null)
            {
                ConsumeToken(Tokens.T_EQ, "=", x.Initializer.Span.StartOrInvalid - 1, x.Span);
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
            ConsumeToken(Tokens.T_LBRACKET, "[", x.KeyExpr.Span.StartOrInvalid - 1, x.Span);
            VisitElement(x.KeyExpr);
            ConsumeToken(Tokens.T_RBRACE, "]", x.KeyExpr.Span.End, x.Span);
        }

        public sealed override void VisitSwitchItem(SwitchItem x)
        {
            throw new InvalidOperationException();  // VisitDefaultItem, VisitCaseItem
        }

        public override void VisitSwitchStmt(SwitchStmt x)
        {
            // switch(VALUE){CASES}
            ConsumeToken(Tokens.T_SWITCH, "switch", x.Span.StartOrInvalid, x.Span);
            // TODO: ENDSWITCH or { }
            ConsumeToken(Tokens.T_LPAREN, "(", x.SwitchValue.Span.StartOrInvalid - 1, x.Span);
            VisitElement(x.SwitchValue);
            ConsumeToken(Tokens.T_RPAREN, ")", x.SwitchValue.Span.End, x.Span);
            ConsumeToken(Tokens.T_LBRACE, "{", x.SwitchValue.Span.End + 1, x.Span);
            VisitList(x.SwitchItems);
            ConsumeToken(Tokens.T_RBRACE, "}", x.Span.End - 1, x.Span);
        }

        public override void VisitThrowStmt(ThrowStmt x)
        {
            // throw EXPR;
            ConsumeToken(Tokens.T_THROW, "throw", x.Span.StartOrInvalid, x.Span);
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
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
        }

        public override void VisitTranslatedTypeRef(TranslatedTypeRef x)
        {
            VisitElement(x.OriginalType);
        }

        public override void VisitTryStmt(TryStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_TRY, "try", x.Span.StartOrInvalid, x.Span);
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
                _composer.ConsumeModifiers(x, x.MemberAttributes);
                if ((x.MemberAttributes & PhpMemberAttributes.Interface) != 0) ConsumeToken(Tokens.T_INTERFACE, "interface", x.Span.StartOrInvalid, x.Span);
                else if ((x.MemberAttributes & PhpMemberAttributes.Trait) != 0) ConsumeToken(Tokens.T_TRAIT, "trait", x.Span.StartOrInvalid, x.Span);
                else ConsumeToken(Tokens.T_CLASS, "class", x.Span.StartOrInvalid, x.Span);

                if (x.Name.HasValue)
                {
                    ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span.StartOrInvalid, x.Span);
                }

                if (x.BaseClass != null)
                {
                    // extends
                    ConsumeToken(Tokens.T_EXTENDS, "extends", x.BaseClass.Span.StartOrInvalid - 8, x.Span);
                    VisitElement((TypeRef)x.BaseClass);
                }

                if (x.ImplementsList != null && x.ImplementsList.Length != 0)
                {
                    // implements|extends
                    if ((x.MemberAttributes & PhpMemberAttributes.Interface) == 0)
                    {
                        ConsumeToken(Tokens.T_IMPLEMENTS, x.ImplementsList[0].Span.StartOrInvalid - 11, x.Span);
                    }
                    else
                    {
                        ConsumeToken(Tokens.T_EXTENDS, x.ImplementsList[0].Span.StartOrInvalid - 8, x.Span);
                    }

                    VisitElementList(x.ImplementsList, VisitNamedTypeRef, Tokens.T_COMMA, ",");
                }


                ConsumeToken(Tokens.T_LBRACE, "{", x.BodySpan.StartOrInvalid, x.Span);
                VisitList(x.Members);
                ConsumeToken(Tokens.T_RBRACE, "}", x.BodySpan.End - 1, x.Span);
            }
        }

        public override void VisitTypeOfEx(TypeOfEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnaryEx(UnaryEx x)
        {
            ConsumeToken(TokenFacts.GetOperationToken(x.Operation), x.OperationPosition, x.Span);
            VisitElement(x.Expr);
        }

        public override void VisitUnsetStmt(UnsetStmt x)
        {
            ConsumeToken(Tokens.T_UNSET, "unset", x.Span.StartOrInvalid, x.Span);
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 5, x.Span);
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 2, x.Span);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        public override void VisitUseStatement(UseStatement x)
        {
            ConsumeToken(Tokens.T_USE, "use", x.Span.StartOrInvalid, x.Span);
            switch (x.Kind)
            {
                case AliasKind.Constant: ConsumeToken(Tokens.T_CONST, "const", x.Span.StartOrInvalid + 4, x.Span); break;
                case AliasKind.Function: ConsumeToken(Tokens.T_FUNCTION, "function", x.Span.StartOrInvalid + 4, x.Span); break;
            }

            VisitElementList(x.Uses, VisitUse, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1, x.Span);
        }

        protected virtual void VisitUse(UseBase use)
        {
            throw new NotImplementedException();
        }

        public override void VisitValueAssignEx(ValueAssignEx x)
        {
            // L = R
            VisitElement(x.LValue);
            ConsumeToken(Tokens.T_EQ, "=", x.LValue.Span.End, x.Span);
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
                ConsumeToken(Tokens.T_WHILE, "while", x.Span.StartOrInvalid, x.Span);
                ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid + 5, x.Span);
                VisitElement(x.CondExpr);
                ConsumeToken(Tokens.T_RPAREN, ")", x.Body.Span.StartOrInvalid - 1, x.Span);
                VisitElement(x.Body);
            }
        }

        public override void VisitYieldEx(YieldEx x)
        {
            ConsumeToken(Tokens.T_YIELD, "yield", x.Span.StartOrInvalid, x.Span);

            if (x.KeyExpr != null)
            {
                VisitElement(x.KeyExpr);
                ConsumeToken(Tokens.T_DOUBLE_ARROW, x.KeyExpr.Span.End, x.Span);
            }

            VisitElement(x.ValueExpr);
        }

        public override void VisitYieldFromEx(YieldFromEx x)
        {
            ConsumeToken(Tokens.T_YIELD_FROM, x.Span.StartOrInvalid, x.Span);
            VisitElement(x.ValueExpr);
        }

        #endregion
    }
}
