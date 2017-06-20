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
        /// <param name="position">Optional. Original token position in source code if provided. Otherwise <c>default(Span)</c>.</param>
        void ConsumeToken(Tokens token, string text, int position = -1);
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
        protected void ConsumeToken(Tokens token, int position = -1) => ConsumeToken(token, TokenFacts.GetTokenText(token), position);

        /// <inheritdoc />
        public virtual void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            if ((modifiers & PhpMemberAttributes.Private) != 0) ConsumeToken(Tokens.T_PRIVATE, span.StartOrInvalid);
            if ((modifiers & PhpMemberAttributes.Protected) != 0) ConsumeToken(Tokens.T_PROTECTED, span.StartOrInvalid);

            if ((modifiers & PhpMemberAttributes.Static) != 0) ConsumeToken(Tokens.T_STATIC, span.StartOrInvalid);
            if ((modifiers & PhpMemberAttributes.Abstract) != 0) ConsumeToken(Tokens.T_ABSTRACT, span.StartOrInvalid);
            if ((modifiers & PhpMemberAttributes.Final) != 0) ConsumeToken(Tokens.T_FINAL, span.StartOrInvalid);
        }

        /// <inheritdoc />
        public virtual void ConsumeToken(Tokens token, string text, int position)
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
                ConsumeToken(Tokens.T_STRING, /*IsUpperCase(x) ? value.ToUpperInvariant() :*/ value.ToLowerInvariant(), literal.Span.StartOrInvalid);
            }
            else if (literal is DoubleLiteral)
            {
                ConsumeToken(Tokens.T_DNUMBER, ((DoubleLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span.StartOrInvalid);
            }
            else if (literal is NullLiteral)
            {
                ConsumeToken(Tokens.T_STRING, /*_composer.IsUpperCase(x) ? "NULL" :*/ "null", literal.Span.StartOrInvalid);
            }
            else if (literal is LongIntLiteral)
            {
                ConsumeToken(Tokens.T_LNUMBER, ((LongIntLiteral)literal).Value.ToString(CultureInfo.InvariantCulture), literal.Span.StartOrInvalid);
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
        public void ConsumeToken(Tokens token, string text, int position = -1)
        {
            _composer.ConsumeToken(token, text, position);
        }

        protected void ConsumeModifiers(LangElement element, PhpMemberAttributes modifiers, Span span)
        {
            _composer.ConsumeModifiers(element, modifiers, span);
        }

        /// <summary>
        /// Shortcut for <see cref="ConsumeToken(Tokens, string, int)"/>.
        /// </summary>
        protected void ConsumeToken(Tokens token, int position = -1) => ConsumeToken(token, TokenFacts.GetTokenText(token), position);

        #region Single Nodes Overrides

        public override void VisitElement(LangElement element)
        {
            base.VisitElement(element);
        }

        public override void VisitActualParam(ActualParam x)
        {
            if (x.IsUnpack) ConsumeToken(Tokens.T_ELLIPSIS, "...");
            if (x.Ampersand) ConsumeToken(Tokens.T_AMP, "&");
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
                ConsumeToken(Tokens.T_ARRAY, "array", x.Span.StartOrInvalid);
                ConsumeToken(Tokens.T_LPAREN, "(");
                VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_RPAREN, ")");
            }
            else
            {
                ConsumeToken(Tokens.T_LBRACKET, "[", x.Span.StartOrInvalid);
                VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_RBRACKET, "]", x.Span.End - 1);
            }
        }

        public override void VisitArrayItem(Item item)
        {
            if (item != null)
            {
                if (item.Index != null)
                {
                    VisitElement(item.Index);
                    ConsumeToken(Tokens.T_DOUBLE_ARROW, "=>");
                }

                if (item is ValueItem)
                {
                    VisitElement(((ValueItem)item).ValueExpr);
                }
                else if (item is RefItem)
                {
                    ConsumeToken(Tokens.T_AMP, "&");
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
            ConsumeToken(Tokens.T_STRING, "assert", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElement(x.CodeEx);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1);
        }

        public sealed override void VisitAssignEx(AssignEx x) { throw new InvalidOperationException(); }

        public override void VisitBinaryEx(BinaryEx x)
        {
            VisitElement(x.LeftExpr);
            ConsumeToken(TokenFacts.GetOperationToken(x.Operation), x.OperationPosition);
            VisitElement(x.RightExpr);
        }

        public override void VisitBinaryStringLiteral(BinaryStringLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitBlockStmt(BlockStmt x)
        {
            ConsumeToken(Tokens.T_LBRACE, "{", x.Span.StartOrInvalid);
            base.VisitBlockStmt(x);
            ConsumeToken(Tokens.T_RBRACE, "}", x.Span.End - 1);
        }

        public override void VisitBoolLiteral(BoolLiteral x)
        {
            _composer.ConsumeLiteral(x);
        }

        public override void VisitCaseItem(CaseItem x)
        {
            ConsumeToken(Tokens.T_CASE, "case");
            VisitElement(x.CaseVal);
            ConsumeToken(Tokens.T_COLON, ":");

            base.VisitCaseItem(x);
        }

        public override void VisitCatchItem(CatchItem x)
        {
            // catch (TYPE VARIABLE) BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_CATCH, "catch");
                ConsumeToken(Tokens.T_LPAREN, "(");
                VisitElement(x.TargetType);
                VisitElement(x.Variable);
                ConsumeToken(Tokens.T_RPAREN, ")");
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
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::");
            ConsumeToken(Tokens.T_STRING, x.Name.Value, x.NamePosition.StartOrInvalid);
        }

        public override void VisitClassTypeRef(ClassTypeRef x)
        {
            VisitQualifiedName(x.ClassName, x.Span.StartOrInvalid);
        }

        public override void VisitConcatEx(ConcatEx x)
        {
            VisitElementList(x.Expressions, Tokens.T_DOT, ".");
        }

        public override void VisitConditionalEx(ConditionalEx x)
        {
            VisitElement(x.CondExpr);
            ConsumeToken(Tokens.T_QUESTION, "?");
            VisitElement(x.TrueExpr);   // can be null
            ConsumeToken(Tokens.T_COLON, ":");
            VisitElement(x.FalseExpr);
        }

        public sealed override void VisitConditionalStmt(ConditionalStmt x)
        {
            throw new InvalidOperationException();  // VisitIfStmt
        }

        public override void VisitConstantDecl(ConstantDecl x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value);

            if (x.Initializer != null)  // always true
            {
                ConsumeToken(Tokens.T_EQ, "=");
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
            ConsumeToken(Tokens.T_CONST, "const");
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";");
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
            ConsumeToken(Tokens.T_DEFAULT, "default");
            ConsumeToken(Tokens.T_COLON, ":");
            VisitList(x.Statements);
        }

        public override void VisitDirectFcnCall(DirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitQualifiedName(x.FullName.OriginalName, x.Span.StartOrInvalid);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectStFldUse(DirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::");
            VisitVariableName(x.PropertyName, x.NameSpan, true);  // $name
        }

        public override void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::");
            ConsumeToken(Tokens.T_STRING, x.MethodName.Name.Value);
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
                ConsumeToken(Tokens.T_INLINE_HTML, ((StringLiteral)x.Parameters[0]).Value, x.Span.StartOrInvalid);
            }
            else
            {
                // echo PARAMETERS;
                ConsumeToken(Tokens.T_ECHO, "echo", x.Span.StartOrInvalid);
                VisitElementList(x.Parameters, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";");
            }
        }

        public override void VisitParenthesisExpression(ParenthesisExpression x)
        {
            ConsumeToken(Tokens.T_LPAREN, "(", x.Span.StartOrInvalid);
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1);
        }

        public override void VisitEmptyEx(EmptyEx x)
        {
            // empty(OPERAND)
            ConsumeToken(Tokens.T_EMPTY, "empty", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1);
        }

        public override void VisitEmptyStmt(EmptyStmt x)
        {
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.StartOrInvalid);
        }

        public override void VisitEvalEx(EvalEx x)
        {
            ConsumeToken(Tokens.T_EVAL, "eval", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElement(x.Code);
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1);
        }

        public override void VisitExitEx(ExitEx x)
        {
            ConsumeToken(Tokens.T_EXIT, "exit", x.Span.StartOrInvalid);
            VisitElement(x.ResulExpr);
        }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            base.VisitExpressionStmt(x);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitFieldDecl(FieldDecl x)
        {
            VisitVariableName(x.Name, x.NameSpan, true);
            if (x.Initializer != null)
            {
                ConsumeToken(Tokens.T_EQ, "=");
                VisitElement(x.Initializer);
            }
        }

        public override void VisitFieldDeclList(FieldDeclList x)
        {
            ConsumeModifiers(x, x.Modifiers, x.Span.IsValid && x.Fields[0].Span.IsValid ? Span.FromBounds(x.Span.StartOrInvalid, x.Fields[0].Span.StartOrInvalid) : Span.Invalid);

            if (x.Modifiers == 0)
            {
                ConsumeToken(Tokens.T_VAR, "var");
            }

            VisitElementList(x.Fields, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            // finally BLOCK
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FINAL, "finally", x.Span.StartOrInvalid);
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                ConsumeToken(Tokens.T_FOREACH, "foreach", x.Span.StartOrInvalid);
                ConsumeToken(Tokens.T_LPAREN, "(");
                VisitElement(x.Enumeree);
                ConsumeToken(Tokens.T_AS, "as");
                if (x.KeyVariable != null)
                {
                    VisitForeachVar(x.KeyVariable);
                    ConsumeToken(Tokens.T_DOUBLE_ARROW, "=>");
                }
                VisitForeachVar(x.ValueVariable);
                ConsumeToken(Tokens.T_RPAREN, ")");
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachVar(ForeachVar x)
        {
            if (x.Alias) ConsumeToken(Tokens.T_AMP, "&", x.Span.StartOrInvalid);
            VisitElement(x.Target);
        }

        public override void VisitFormalParam(FormalParam x)
        {
            VisitElement(x.TypeHint);
            if (x.PassedByRef)
            {
                ConsumeToken(Tokens.T_AMP, "&");
            }
            if (x.IsVariadic)
            {
                ConsumeToken(Tokens.T_ELLIPSIS, "...");
            }

            VisitVariableName(x.Name.Name, x.Name.Span, true);
            if (x.InitValue != null)
            {
                ConsumeToken(Tokens.T_EQ, "=");
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
                ConsumeToken(Tokens.T_FOR, "for", x.Span.StartOrInvalid);
                ConsumeToken(Tokens.T_LPAREN, "(");

                VisitElementList(x.InitExList, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";");
                VisitElementList(x.CondExList, Tokens.T_COMMA, ",");
                ConsumeToken(Tokens.T_SEMI, ";");
                VisitElementList(x.ActionExList, Tokens.T_COMMA, ",");

                ConsumeToken(Tokens.T_LPAREN, ")");

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
                ConsumeToken(Tokens.T_FUNCTION, "function");
                if (signature.AliasReturn) ConsumeToken(Tokens.T_AMP, "&");
                if (nameOpt.HasValue) ConsumeToken(Tokens.T_STRING, nameOpt.Name.Value);
                VisitSignature(signature);
                if (returnTypeOpt != null)
                {
                    ConsumeToken(Tokens.T_COLON, ":");
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
            ConsumeToken(Tokens.T_CONST, "const");
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitGlobalConstUse(GlobalConstUse x)
        {
            VisitQualifiedName(x.FullName.OriginalName, x.Span.StartOrInvalid);
        }

        public override void VisitGlobalStmt(GlobalStmt x)
        {
            ConsumeToken(Tokens.T_GLOBAL, x.Span.StartOrInvalid);
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitGotoStmt(GotoStmt x)
        {
            ConsumeToken(Tokens.T_GOTO, "goto", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_STRING, x.LabelName.Name.Value);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitHaltCompiler(HaltCompiler x)
        {
            ConsumeToken(Tokens.T_HALT_COMPILER, "__halt_compiler", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_LPAREN, "(");
            ConsumeToken(Tokens.T_RPAREN, ")");
            ConsumeToken(Tokens.T_SEMI, ";");
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
                        ConsumeToken(Tokens.T_IF, cond.Span.StartOrInvalid);
                    }
                    else if (cond.Condition != null)
                    {
                        ConsumeToken(Tokens.T_ELSEIF, cond.Span.StartOrInvalid);
                    }
                    else
                    {
                        ConsumeToken(Tokens.T_ELSE, cond.Span.StartOrInvalid);
                    }

                    if (cond.Condition != null)
                    {
                        ConsumeToken(Tokens.T_LPAREN, "(");
                        VisitElement(cond.Condition);
                        ConsumeToken(Tokens.T_RPAREN, ")");
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
            ConsumeToken(x.Inc ? Tokens.T_INC : Tokens.T_DEC, x.Inc ? "++" : "--");

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
                    ConsumeToken(Tokens.T_INCLUDE, "include", x.Span.StartOrInvalid);
                    break;
                case InclusionTypes.IncludeOnce:
                    ConsumeToken(Tokens.T_INCLUDE_ONCE, "include_once", x.Span.StartOrInvalid);
                    break;
                case InclusionTypes.Require:
                    ConsumeToken(Tokens.T_REQUIRE, "require", x.Span.StartOrInvalid);
                    break;
                case InclusionTypes.RequireOnce:
                    ConsumeToken(Tokens.T_REQUIRE_ONCE, "require_once", x.Span.StartOrInvalid);
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
            ConsumeToken(Tokens.T_VARIABLE, (dollar ? "$" : string.Empty) + name.Value, span.StartOrInvalid);
        }

        public virtual void VisitQualifiedName(QualifiedName qname, int position)
        {
            if (qname.IsFullyQualifiedName)
            {
                ConsumeToken(Tokens.T_NS_SEPARATOR, QualifiedName.Separator.ToString(), position);
            }

            var ns = qname.Namespaces;
            for (int i = 0; i < ns.Length; i++)
            {
                ConsumeToken(Tokens.T_STRING, ns[i].Value, position);
                ConsumeToken(Tokens.T_NS_SEPARATOR, QualifiedName.Separator.ToString(), position);
            }

            ConsumeToken(Tokens.T_STRING, qname.Name.Value, position);
        }

        public virtual void VisitCallSignature(CallSignature signature)
        {
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElementList(signature.Parameters, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")");
        }

        public virtual void VisitSignature(Signature signature)
        {
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElementList(signature.FormalParams, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")");
        }

        public override void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::");
            VisitElement(x.FieldNameExpr);  // TODO: { ... } ?
        }

        public override void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::");
            VisitElement(x.MethodNameVar);  // TODO: { ... } ?
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
            ConsumeToken(Tokens.T_INSTANCEOF, "instanceof");
            VisitElement(x.ClassNameRef);
        }

        public override void VisitIssetEx(IssetEx x)
        {
            ConsumeToken(Tokens.T_ISSET, "isset", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")");
        }

        public override void VisitItemUse(ItemUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitElement(x.Array);
            ConsumeToken(Tokens.T_LBRACKET, "[");
            VisitElement(x.Index);
            ConsumeToken(Tokens.T_RBRACKET, "]");
        }

        public virtual void VisitIsMemberOf(Expression isMemberOf)
        {
            if (isMemberOf != null)
            {
                VisitElement(isMemberOf);
                ConsumeToken(Tokens.T_OBJECT_OPERATOR, "->");
            }
        }

        public override void VisitJumpStmt(JumpStmt x)
        {
            switch (x.Type)
            {
                case JumpStmt.Types.Return:
                    ConsumeToken(Tokens.T_RETURN, "return", x.Span.StartOrInvalid);
                    break;
                case JumpStmt.Types.Continue:
                    ConsumeToken(Tokens.T_CONTINUE, "continue", x.Span.StartOrInvalid);
                    break;
                case JumpStmt.Types.Break:
                    ConsumeToken(Tokens.T_BREAK, "break", x.Span.StartOrInvalid);
                    break;
            }

            VisitElement(x.Expression);

            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitLabelStmt(LabelStmt x)
        {
            ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_COLON, ":", x.Span.End - 1);
        }

        public override void VisitLambdaFunctionExpr(LambdaFunctionExpr x)
        {
            VisitRoutineDecl(x, x.Signature, x.Body,
                returnTypeOpt: x.ReturnType,
                modifiers: x.IsStatic ? PhpMemberAttributes.Static : PhpMemberAttributes.None);
        }

        public override void VisitListEx(ListEx x)
        {
            ConsumeToken(Tokens.T_LIST, "list", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")", x.Span.End - 1);
        }

        protected virtual void VisitElementList<TElement>(IList<TElement> list, Tokens separatorToken, string separatorTokenText) where TElement : LangElement
        {
            VisitElementList(list, VisitElement, separatorToken, separatorTokenText);
        }

        protected virtual void VisitElementList<TElement>(IList<TElement> list, Action<TElement> action, Tokens separatorToken, string separatorTokenText)
        {
            Debug.Assert(list != null, nameof(list));
            Debug.Assert(action != null, nameof(action));

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0) ConsumeToken(separatorToken, separatorTokenText);
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
            ConsumeToken(Tokens.T_NAMESPACE, "namespace", x.Span.StartOrInvalid);

            if (x.QualifiedName.HasValue)
            {
                VisitQualifiedName(x.QualifiedName.QualifiedName.WithFullyQualified(false), x.Span.StartOrInvalid);
            }

            if (x.IsSimpleSyntax)
            {
                // namespace QNAME; BODY
                ConsumeToken(Tokens.T_SEMI, ";");
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
            ConsumeToken(Tokens.T_NEW, "new");
            VisitElement(x.ClassNameRef);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitNullableTypeRef(NullableTypeRef x)
        {
            ConsumeToken(Tokens.T_QUESTION, "?");
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
            VisitQualifiedName(x.QualifiedName.Value, x.Span.StartOrInvalid);
        }

        public override void VisitPseudoClassConstUse(PseudoClassConstUse x)
        {
            VisitElement(x.TargetType);
            ConsumeToken(Tokens.T_DOUBLE_COLON, "::");
            switch (x.Type)
            {
                case PseudoClassConstUse.Types.Class:
                    ConsumeToken(Tokens.T_CLASS, "class", x.NamePosition.StartOrInvalid);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public override void VisitPseudoConstUse(PseudoConstUse x)
        {
            ConsumeToken(TokenFacts.GetPseudoConstUseToken(x.Type), x.Span.StartOrInvalid);
        }

        public override void VisitRefAssignEx(RefAssignEx x)
        {
            // L =& R
            VisitElement(x.LValue);
            ConsumeToken(Tokens.T_EQ, "=");
            ConsumeToken(Tokens.T_AMP, "&");
            VisitElement(x.RValue);
        }

        public sealed override void VisitRefItem(RefItem x)
        {
            throw new InvalidOperationException(); // VisitArrayItem
        }

        public override void VisitReservedTypeRef(ReservedTypeRef x)
        {
            VisitQualifiedName(x.QualifiedName.Value, x.Span.StartOrInvalid);
        }

        public override void VisitShellEx(ShellEx x)
        {
            ConsumeToken(Tokens.T_BACKQUOTE, x.Span.StartOrInvalid);
            // content
            ConsumeToken(Tokens.T_BACKQUOTE, x.Span.End - 1);
            throw new NotImplementedException();
        }

        public override void VisitStaticStmt(StaticStmt x)
        {
            ConsumeToken(Tokens.T_STATIC, "static", x.Span.StartOrInvalid);
            VisitElementList(x.StVarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitStaticVarDecl(StaticVarDecl x)
        {
            VisitVariableName(x.Variable, x.NameSpan, true);

            if (x.Initializer != null)
            {
                ConsumeToken(Tokens.T_EQ, "=");
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
            ConsumeToken(Tokens.T_LBRACKET, "[");
            VisitElement(x.KeyExpr);
            ConsumeToken(Tokens.T_RBRACE, "]");
        }

        public sealed override void VisitSwitchItem(SwitchItem x)
        {
            throw new InvalidOperationException();  // VisitDefaultItem, VisitCaseItem
        }

        public override void VisitSwitchStmt(SwitchStmt x)
        {
            // switch(VALUE){CASES}
            ConsumeToken(Tokens.T_SWITCH, "switch", x.Span.StartOrInvalid);
            // TODO: ENDSWITCH or { }
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElement(x.SwitchValue);
            ConsumeToken(Tokens.T_RPAREN, ")");
            ConsumeToken(Tokens.T_LBRACE, "{");
            VisitList(x.SwitchItems);
            ConsumeToken(Tokens.T_RBRACE, "}");
        }

        public override void VisitThrowStmt(ThrowStmt x)
        {
            // throw EXPR;
            ConsumeToken(Tokens.T_THROW, "throw", x.Span.StartOrInvalid);
            VisitElement(x.Expression);
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
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
                ConsumeToken(Tokens.T_TRY, "try", x.Span.StartOrInvalid);
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
                if ((x.MemberAttributes & PhpMemberAttributes.Interface) != 0) ConsumeToken(Tokens.T_INTERFACE, "interface");
                else if ((x.MemberAttributes & PhpMemberAttributes.Trait) != 0) ConsumeToken(Tokens.T_TRAIT, "trait");
                else ConsumeToken(Tokens.T_CLASS, "class");

                if (x.Name.HasValue)
                {
                    ConsumeToken(Tokens.T_STRING, x.Name.Name.Value, x.Name.Span.StartOrInvalid);
                }

                if (x.BaseClass != null)
                {
                    // extends
                    ConsumeToken(Tokens.T_EXTENDS, "extends");
                    VisitElement((TypeRef)x.BaseClass);
                }

                if (x.ImplementsList != null && x.ImplementsList.Length != 0)
                {
                    // implements|extends
                    if ((x.MemberAttributes & PhpMemberAttributes.Interface) == 0)
                    {
                        ConsumeToken(Tokens.T_IMPLEMENTS);
                    }
                    else
                    {
                        ConsumeToken(Tokens.T_EXTENDS);
                    }

                    VisitElementList(x.ImplementsList, VisitNamedTypeRef, Tokens.T_COMMA, ",");
                }


                ConsumeToken(Tokens.T_LBRACE, "{", x.BodySpan.StartOrInvalid);
                VisitList(x.Members);
                ConsumeToken(Tokens.T_RBRACE, "}", x.BodySpan.End - 1);
            }
        }

        public override void VisitTypeOfEx(TypeOfEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnaryEx(UnaryEx x)
        {
            ConsumeToken(TokenFacts.GetOperationToken(x.Operation), x.OperationPosition);
            VisitElement(x.Expr);
        }

        public override void VisitUnsetStmt(UnsetStmt x)
        {
            ConsumeToken(Tokens.T_UNSET, "unset", x.Span.StartOrInvalid);
            ConsumeToken(Tokens.T_LPAREN, "(");
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_RPAREN, ")");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        public override void VisitUseStatement(UseStatement x)
        {
            ConsumeToken(Tokens.T_USE, "use", x.Span.StartOrInvalid);
            switch (x.Kind)
            {
                case AliasKind.Constant: ConsumeToken(Tokens.T_CONST, "const"); break;
                case AliasKind.Function: ConsumeToken(Tokens.T_FUNCTION, "function"); break;
            }

            VisitElementList(x.Uses, VisitUse, Tokens.T_COMMA, ",");
            ConsumeToken(Tokens.T_SEMI, ";", x.Span.End - 1);
        }

        protected virtual void VisitUse(UseBase use)
        {
            throw new NotImplementedException();
        }

        public override void VisitValueAssignEx(ValueAssignEx x)
        {
            // L = R
            VisitElement(x.LValue);
            ConsumeToken(Tokens.T_EQ, "=");
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
                ConsumeToken(Tokens.T_WHILE, "while", x.Span.StartOrInvalid);
                ConsumeToken(Tokens.T_LPAREN, "(");
                VisitElement(x.CondExpr);
                ConsumeToken(Tokens.T_RPAREN, ")");
                VisitElement(x.Body);
            }
        }

        public override void VisitYieldEx(YieldEx x)
        {
            ConsumeToken(Tokens.T_YIELD, "yield", x.Span.StartOrInvalid);

            if (x.KeyExpr != null)
            {
                VisitElement(x.KeyExpr);
                ConsumeToken(Tokens.T_DOUBLE_ARROW);
            }

            VisitElement(x.ValueExpr);
        }

        public override void VisitYieldFromEx(YieldFromEx x)
        {
            ConsumeToken(Tokens.T_YIELD_FROM, x.Span.StartOrInvalid);
            VisitElement(x.ValueExpr);
        }

        #endregion
    }
}
