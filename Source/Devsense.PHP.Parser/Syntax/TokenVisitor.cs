using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax.Ast;
using System.Diagnostics;
using System.Globalization;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Options specifying how <see cref="TokenVisitor"/> synthesizes tokens from the syntax tree if not specified.
    /// </summary>
    public interface ITokenVisitorOptions
    {
        /// <summary>
        /// Gets value indicating the literal (null, true, false) is uppercased.
        /// </summary>
        bool IsUpperCase(Literal literal);

        /// <summary>
        /// Whether to tokenize according to the old syntax <code>array(...)</code> or the new syntax <code>[...]</code>
        /// </summary>
        bool IsOldArraySyntax(ArrayEx node);
    }

    public class TokenVisitor : TreeContextVisitor
    {
        #region DefaultTokenVisitorOptions

        /// <summary>
        /// A default options implementation with default values.
        /// </summary>
        sealed class DefaultTokenVisitorOptions : ITokenVisitorOptions
        {
            public static ITokenVisitorOptions Instance = new DefaultTokenVisitorOptions();

            private DefaultTokenVisitorOptions() { }

            public bool IsOldArraySyntax(ArrayEx node) => false;

            public bool IsUpperCase(Literal literal) => false;
        }

        #endregion

        readonly ITokenVisitorOptions _options;

        public TokenVisitor(TreeContext initialContext, ITokenVisitorOptions options = null) : base(initialContext)
        {
            _options = options ?? DefaultTokenVisitorOptions.Instance;  // TODO: to TreeContext
        }

        /// <summary>
        /// Invoked when a token is visited.
        /// </summary>
        /// <param name="token">Token id.</param>
        /// <param name="text">Textual representation of <paramref name="token"/>.</param>
        /// <param name="semantic">Optional token semantic value.
        /// In case of string literals, numbers or comments, this specifies its original representation in source code.</param>
        protected virtual void VisitToken(Tokens token, string text, object semantic = null)
        {

        }

        /// <summary>
        /// Shortcut for <see cref="VisitToken(Tokens, string, object)"/>.
        /// </summary>
        protected void VisitToken(Tokens token) => VisitToken(token, TokenFacts.GetTokenText(token));

        #region Single Nodes Overrides

        public override void VisitElement(LangElement element)
        {
            base.VisitElement(element);
        }

        public override void VisitActualParam(ActualParam x)
        {
            if (x.IsUnpack) VisitToken(Tokens.T_ELLIPSIS, "...");
            if (x.Ampersand) VisitToken(Tokens.T_AMP, "&");
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
            if (_options.IsOldArraySyntax(x))
            {
                VisitToken(Tokens.T_ARRAY, "array");
                VisitToken(Tokens.T_LPAREN, "(");
                VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
                VisitToken(Tokens.T_RPAREN, ")");
            }
            else
            {
                VisitToken(Tokens.T_LBRACKET, "[");
                VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
                VisitToken(Tokens.T_RBRACKET, "]");
            }
        }

        public override void VisitArrayItem(Item item)
        {
            if (item != null)
            {
                if (item.Index != null)
                {
                    VisitElement(item.Index);
                    VisitToken(Tokens.T_DOUBLE_ARROW, "=>");
                }

                if (item is ValueItem)
                {
                    VisitElement(((ValueItem)item).ValueExpr);
                }
                else if (item is RefItem)
                {
                    VisitToken(Tokens.T_AMP, "&");
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
            VisitToken(Tokens.T_STRING, "assert");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElement(x.CodeEx);
            VisitToken(Tokens.T_RPAREN, ")");
        }

        public sealed override void VisitAssignEx(AssignEx x) { throw new InvalidOperationException(); }

        public override void VisitBinaryEx(BinaryEx x)
        {
            VisitElement(x.LeftExpr);
            VisitToken(TokenFacts.GetOperationToken(x.Operation));
            VisitElement(x.RightExpr);
        }

        public override void VisitBinaryStringLiteral(BinaryStringLiteral x)
        {
            throw new NotImplementedException();
        }

        public override void VisitBlockStmt(BlockStmt x)
        {
            VisitToken(Tokens.T_LBRACE, "{");
            base.VisitBlockStmt(x);
            VisitToken(Tokens.T_RBRACE, "}");
        }

        public override void VisitBoolLiteral(BoolLiteral x)
        {
            var value = x.Value.ToString();
            VisitToken(Tokens.T_STRING, _options.IsUpperCase(x) ? value.ToUpperInvariant() : value.ToLowerInvariant());
        }

        public override void VisitCaseItem(CaseItem x)
        {
            VisitToken(Tokens.T_CASE, "case");
            VisitElement(x.CaseVal);
            VisitToken(Tokens.T_COLON, ":");

            base.VisitCaseItem(x);
        }

        public override void VisitCatchItem(CatchItem x)
        {
            // catch (TYPE VARIABLE) BLOCK
            using (new ScopeHelper(this, x))
            {
                VisitToken(Tokens.T_CATCH, "catch");
                VisitToken(Tokens.T_LPAREN, "(");
                VisitElement(x.TargetType);
                VisitElement(x.Variable);
                VisitToken(Tokens.T_RPAREN, ")");
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
            VisitToken(Tokens.T_DOUBLE_COLON, "::");
            VisitToken(Tokens.T_STRING, x.Name.Value);
        }

        public override void VisitClassTypeRef(ClassTypeRef x)
        {
            VisitQualifiedName(x.ClassName);
        }

        public override void VisitConcatEx(ConcatEx x)
        {
            VisitElementList(x.Expressions, Tokens.T_DOT, ".");
        }

        public override void VisitConditionalEx(ConditionalEx x)
        {
            VisitElement(x.CondExpr);
            VisitToken(Tokens.T_QUESTION, "?");
            VisitElement(x.TrueExpr);   // can be null
            VisitToken(Tokens.T_COLON, ":");
            VisitElement(x.FalseExpr);
        }

        public sealed override void VisitConditionalStmt(ConditionalStmt x)
        {
            throw new InvalidOperationException();  // VisitIfStmt
        }

        public override void VisitConstantDecl(ConstantDecl x)
        {
            VisitToken(Tokens.T_STRING, x.Name.Name.Value);

            if (x.Initializer != null)  // always true
            {
                VisitToken(Tokens.T_EQ, "=");
                VisitElement(x.Initializer);
            }
        }

        public sealed override void VisitConstantUse(ConstantUse x)
        {
            throw new InvalidOperationException(); // override PseudoConstant, ClassCOnstant, GlobalConstant
        }

        protected virtual void VisitModifiers(LangElement element, PhpMemberAttributes attrs)
        {
            // TODO: order

            if ((attrs & PhpMemberAttributes.Private) != 0) VisitToken(Tokens.T_PRIVATE);
            if ((attrs & PhpMemberAttributes.Protected) != 0) VisitToken(Tokens.T_PROTECTED);
            // TODO: public ?

            if ((attrs & PhpMemberAttributes.Static) != 0) VisitToken(Tokens.T_STATIC);
            if ((attrs & PhpMemberAttributes.Abstract) != 0) VisitToken(Tokens.T_ABSTRACT);
            if ((attrs & PhpMemberAttributes.Final) != 0) VisitToken(Tokens.T_FINAL);
        }

        public override void VisitConstDeclList(ConstDeclList x)
        {
            VisitModifiers(x, x.Modifiers);
            VisitToken(Tokens.T_CONST, "const");
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_SEMI, ";");
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
            VisitToken(Tokens.T_DEFAULT, "default");
            VisitToken(Tokens.T_COLON, ":");
            VisitList(x.Statements);
        }

        public override void VisitDirectFcnCall(DirectFcnCall x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitQualifiedName(x.FullName.OriginalName);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectStFldUse(DirectStFldUse x)
        {
            VisitElement(x.TargetType);
            VisitToken(Tokens.T_DOUBLE_COLON, "::");
            VisitVariableName(x.PropertyName);  // $name
        }

        public override void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            VisitToken(Tokens.T_DOUBLE_COLON, "::");
            VisitToken(Tokens.T_STRING, x.MethodName.Name.Value);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitDirectVarUse(DirectVarUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitVariableName(x.VarName);
        }

        public override void VisitDoubleLiteral(DoubleLiteral x)
        {
            VisitToken(Tokens.T_DNUMBER, x.Value.ToString(CultureInfo.InvariantCulture));
        }

        public override void VisitEchoStmt(EchoStmt x)
        {
            if (x.IsHtmlCode)
            {
                VisitToken(Tokens.T_INLINE_HTML, ((StringLiteral)x.Parameters[0]).Value);
            }
            else
            {
                // echo PARAMETERS;
                VisitToken(Tokens.T_ECHO, "echo");
                VisitElementList(x.Parameters, Tokens.T_COMMA, ",");
                VisitToken(Tokens.T_SEMI, ";");
            }
        }

        public override void VisitEmptyEx(EmptyEx x)
        {
            // empty(OPERAND)
            VisitToken(Tokens.T_EMPTY, "empty");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElement(x.Expression);
            VisitToken(Tokens.T_RPAREN, ")");
        }

        public override void VisitEmptyStmt(EmptyStmt x)
        {
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitEvalEx(EvalEx x)
        {
            VisitToken(Tokens.T_EVAL, "eval");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElement(x.Code);
            VisitToken(Tokens.T_RPAREN, ")");
        }

        public override void VisitExitEx(ExitEx x)
        {
            VisitToken(Tokens.T_EXIT, "exit");
            VisitElement(x.ResulExpr);
        }

        public override void VisitExpressionStmt(ExpressionStmt x)
        {
            base.VisitExpressionStmt(x);
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitFieldDecl(FieldDecl x)
        {
            VisitVariableName(x.Name);
            if (x.Initializer != null)
            {
                VisitToken(Tokens.T_EQ, "=");
                VisitElement(x.Initializer);
            }
        }

        public override void VisitFieldDeclList(FieldDeclList x)
        {
            VisitModifiers(x, x.Modifiers);

            if (x.Modifiers == 0)
            {
                VisitToken(Tokens.T_VAR, "var");
            }

            VisitElementList(x.Fields, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            // finally BLOCK
            using (new ScopeHelper(this, x))
            {
                VisitToken(Tokens.T_FINAL, "finally");
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                VisitToken(Tokens.T_FOREACH, "foreach");
                VisitToken(Tokens.T_LPAREN, "(");
                VisitElement(x.Enumeree);
                VisitToken(Tokens.T_AS, "as");
                if (x.KeyVariable != null)
                {
                    VisitForeachVar(x.KeyVariable);
                    VisitToken(Tokens.T_DOUBLE_ARROW, "=>");
                }
                VisitForeachVar(x.ValueVariable);
                VisitToken(Tokens.T_RPAREN, ")");
                VisitElement(x.Body);
            }
        }

        public override void VisitForeachVar(ForeachVar x)
        {
            if (x.Alias) VisitToken(Tokens.T_AMP, "&");
            VisitElement(x.Target);
        }

        public override void VisitFormalParam(FormalParam x)
        {
            VisitElement(x.TypeHint);
            if (x.PassedByRef)
            {
                VisitToken(Tokens.T_AMP, "&");
            }
            if (x.IsVariadic)
            {
                VisitToken(Tokens.T_ELLIPSIS, "...");
            }

            VisitVariableName(x.Name.Name);
        }

        public override void VisitFormalTypeParam(FormalTypeParam x)
        {
            throw new NotImplementedException();
        }

        public override void VisitForStmt(ForStmt x)
        {
            using (new ScopeHelper(this, x))
            {
                VisitToken(Tokens.T_FOR, "for");
                VisitToken(Tokens.T_LPAREN, "(");

                VisitElementList(x.InitExList, Tokens.T_COMMA, ",");
                VisitToken(Tokens.T_SEMI, ";");
                VisitElementList(x.CondExList, Tokens.T_COMMA, ",");
                VisitToken(Tokens.T_SEMI, ";");
                VisitElementList(x.ActionExList, Tokens.T_COMMA, ",");

                VisitToken(Tokens.T_LPAREN, ")");

                VisitElement(x.Body);
            }
        }

        public sealed override void VisitFunctionCall(FunctionCall x)
        {
            throw new InvalidOperationException(); // DirectFncCall, IndirectFncCall, *MethodCall, ...
        }

        protected virtual void VisitRoutineDecl(LangElement element, Signature signature, Statement body, string nameOpt = null, TypeRef returnTypeOpt = null, PhpMemberAttributes modifiers = PhpMemberAttributes.None)
        {
            using (new ScopeHelper(this, element))
            {
                // function &NAME SIGNATURE : RETURN_TYPE BODY
                VisitModifiers(element, modifiers);
                VisitToken(Tokens.T_FUNCTION, "function");
                if (signature.AliasReturn) VisitToken(Tokens.T_AMP, "&");
                if (nameOpt != null) VisitToken(Tokens.T_STRING, nameOpt);
                VisitSignature(signature);
                if (returnTypeOpt != null)
                {
                    VisitToken(Tokens.T_COLON, ":");
                    VisitElement(returnTypeOpt);
                }
                VisitElement(body);
            }
        }

        public override void VisitFunctionDecl(FunctionDecl x)
        {
            VisitRoutineDecl(x, x.Signature, x.Body, x.Name.Name.Value, x.ReturnType);
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
            VisitToken(Tokens.T_CONST, "const");
            VisitElementList(x.Constants, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitGlobalConstUse(GlobalConstUse x)
        {
            VisitQualifiedName(x.FullName.OriginalName);
        }

        public override void VisitGlobalStmt(GlobalStmt x)
        {
            VisitToken(Tokens.T_GLOBAL);
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitGotoStmt(GotoStmt x)
        {
            VisitToken(Tokens.T_GOTO, "goto");
            VisitToken(Tokens.T_STRING, x.LabelName.Name.Value);
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitHaltCompiler(HaltCompiler x)
        {
            VisitToken(Tokens.T_HALT_COMPILER, "__halt_compiler");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitToken(Tokens.T_RPAREN, ")");
            VisitToken(Tokens.T_SEMI, ";");
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
                        VisitToken(Tokens.T_IF);
                    }
                    else if (cond.Condition != null)
                    {
                        VisitToken(Tokens.T_ELSEIF);
                    }
                    else
                    {
                        VisitToken(Tokens.T_ELSE);
                    }

                    if (cond.Condition != null)
                    {
                        VisitToken(Tokens.T_LPAREN, "(");
                        VisitElement(cond.Condition);
                        VisitToken(Tokens.T_RPAREN, ")");
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
            VisitToken(x.Inc ? Tokens.T_INC : Tokens.T_DEC, x.Inc ? "++" : "--");

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
                    VisitToken(Tokens.T_INCLUDE, "include");
                    break;
                case InclusionTypes.IncludeOnce:
                    VisitToken(Tokens.T_INCLUDE_ONCE, "include_once");
                    break;
                case InclusionTypes.Require:
                    VisitToken(Tokens.T_REQUIRE, "require");
                    break;
                case InclusionTypes.RequireOnce:
                    VisitToken(Tokens.T_REQUIRE_ONCE, "require_once");
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

        public virtual void VisitVariableName(VariableName name)
        {
            VisitToken(Tokens.T_VARIABLE, "$" + name.Value);
        }

        public virtual void VisitQualifiedName(QualifiedName qname)
        {
            if (qname.IsFullyQualifiedName)
            {
                VisitToken(Tokens.T_NS_SEPARATOR, QualifiedName.Separator.ToString());
            }

            var ns = qname.Namespaces;
            for (int i = 0; i < ns.Length; i++)
            {
                VisitToken(Tokens.T_STRING, ns[i].Value);
                VisitToken(Tokens.T_NS_SEPARATOR, QualifiedName.Separator.ToString());
            }

            VisitToken(Tokens.T_STRING, qname.Name.Value);
        }

        public virtual void VisitCallSignature(CallSignature signature)
        {
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElementList(signature.Parameters, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_RPAREN, ")");
        }

        public virtual void VisitSignature(Signature signature)
        {
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElementList(signature.FormalParams, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_RPAREN, ")");
        }

        public override void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            VisitElement(x.TargetType);
            VisitToken(Tokens.T_DOUBLE_COLON, "::");
            VisitElement(x.FieldNameExpr);  // TODO: { ... } ?
        }

        public override void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            VisitToken(Tokens.T_DOUBLE_COLON, "::");
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
            VisitToken(Tokens.T_INSTANCEOF, "instanceof");
            VisitElement(x.ClassNameRef);
        }

        public override void VisitIssetEx(IssetEx x)
        {
            VisitToken(Tokens.T_ISSET, "isset");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_RPAREN, ")");
        }

        public override void VisitItemUse(ItemUse x)
        {
            VisitIsMemberOf(x.IsMemberOf);
            VisitElement(x.Array);
            VisitToken(Tokens.T_LBRACKET, "[");
            VisitElement(x.Index);
            VisitToken(Tokens.T_RBRACKET, "]");
        }

        public virtual void VisitIsMemberOf(Expression isMemberOf)
        {
            if (isMemberOf != null)
            {
                VisitElement(isMemberOf);
                VisitToken(Tokens.T_OBJECT_OPERATOR, "->");
            }
        }

        public override void VisitJumpStmt(JumpStmt x)
        {
            switch (x.Type)
            {
                case JumpStmt.Types.Return:
                    VisitToken(Tokens.T_RETURN, "return");
                    break;
                case JumpStmt.Types.Continue:
                    VisitToken(Tokens.T_CONTINUE, "continue");
                    break;
                case JumpStmt.Types.Break:
                    VisitToken(Tokens.T_BREAK, "break");
                    break;
            }

            VisitElement(x.Expression);

            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitLabelStmt(LabelStmt x)
        {
            VisitToken(Tokens.T_STRING, x.Name.Name.Value);
            VisitToken(Tokens.T_COLON, ":");
        }

        public override void VisitLambdaFunctionExpr(LambdaFunctionExpr x)
        {
            VisitRoutineDecl(x, x.Signature, x.Body,
                returnTypeOpt: x.ReturnType,
                modifiers: x.IsStatic ? PhpMemberAttributes.Static : PhpMemberAttributes.None);
        }

        public override void VisitListEx(ListEx x)
        {
            VisitToken(Tokens.T_LIST, "list");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElementList(x.Items, VisitArrayItem, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_RPAREN, ")");
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
                if (i != 0) VisitToken(separatorToken, separatorTokenText);
                action(list[i]);
            }
        }

        private void VisitNamedTypeRef(INamedTypeRef tref) => VisitElement((TypeRef)tref);

        public override void VisitLongIntLiteral(LongIntLiteral x)
        {
            VisitToken(Tokens.T_LNUMBER, x.Value.ToString(CultureInfo.InvariantCulture));
        }

        public override void VisitMethodDecl(MethodDecl x)
        {
            VisitRoutineDecl(x, x.Signature, x.Body, x.Name.Name.Value, x.ReturnType, x.Modifiers);
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
            VisitToken(Tokens.T_NAMESPACE, "namespace");

            if (x.QualifiedName.HasValue)
            {
                VisitQualifiedName(x.QualifiedName.QualifiedName.WithFullyQualified(false));
            }

            if (x.IsSimpleSyntax)
            {
                // namespace QNAME; BODY
                VisitToken(Tokens.T_SEMI, ";");
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
            VisitToken(Tokens.T_NEW, "new");
            VisitElement(x.ClassNameRef);
            VisitCallSignature(x.CallSignature);
        }

        public override void VisitNullableTypeRef(NullableTypeRef x)
        {
            VisitToken(Tokens.T_QUESTION, "?");
            VisitElement(x.TargetType);
        }

        public override void VisitNullLiteral(NullLiteral x)
        {
            VisitToken(Tokens.T_STRING, _options.IsUpperCase(x) ? "NULL" : "null");
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
            VisitQualifiedName(x.QualifiedName.Value);
        }

        public override void VisitPseudoClassConstUse(PseudoClassConstUse x)
        {
            VisitElement(x.TargetType);
            VisitToken(Tokens.T_DOUBLE_COLON, "::");
            switch (x.Type)
            {
                case PseudoClassConstUse.Types.Class:
                    VisitToken(Tokens.T_CLASS, "class");
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public override void VisitPseudoConstUse(PseudoConstUse x)
        {
            VisitToken(TokenFacts.GetPseudoConstUseToken(x.Type));
        }

        public override void VisitRefAssignEx(RefAssignEx x)
        {
            // L =& R
            VisitElement(x.LValue);
            VisitToken(Tokens.T_EQ, "=");
            VisitToken(Tokens.T_AMP, "&");
            VisitElement(x.RValue);
        }

        public sealed override void VisitRefItem(RefItem x)
        {
            throw new InvalidOperationException(); // VisitArrayItem
        }

        public override void VisitReservedTypeRef(ReservedTypeRef x)
        {
            VisitQualifiedName(x.QualifiedName.Value);
        }

        public override void VisitShellEx(ShellEx x)
        {
            VisitToken(Tokens.T_BACKQUOTE);
            // content
            VisitToken(Tokens.T_BACKQUOTE);
            throw new NotImplementedException();
        }

        public override void VisitStaticStmt(StaticStmt x)
        {
            VisitToken(Tokens.T_STATIC, "static");
            VisitElementList(x.StVarList, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitStaticVarDecl(StaticVarDecl x)
        {
            VisitVariableName(x.Variable);

            if (x.Initializer != null)
            {
                VisitToken(Tokens.T_EQ, "=");
                VisitElement(x.Initializer);
            }
        }

        public override void VisitStringLiteral(StringLiteral x)
        {
            throw new NotImplementedException();
        }

        public override void VisitStringLiteralDereferenceEx(StringLiteralDereferenceEx x)
        {
            VisitElement(x.StringExpr);
            VisitToken(Tokens.T_LBRACKET, "[");
            VisitElement(x.KeyExpr);
            VisitToken(Tokens.T_RBRACE, "]");
        }

        public sealed override void VisitSwitchItem(SwitchItem x)
        {
            throw new InvalidOperationException();  // VisitDefaultItem, VisitCaseItem
        }

        public override void VisitSwitchStmt(SwitchStmt x)
        {
            // switch(VALUE){CASES}
            VisitToken(Tokens.T_SWITCH, "switch");
            // TODO: ENDSWITCH or { }
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElement(x.SwitchValue);
            VisitToken(Tokens.T_RPAREN, ")");
            VisitToken(Tokens.T_LBRACE, "{");
            VisitList(x.SwitchItems);
            VisitToken(Tokens.T_RBRACE, "}");
        }

        public override void VisitThrowStmt(ThrowStmt x)
        {
            // throw EXPR;
            VisitToken(Tokens.T_THROW, "throw");
            VisitElement(x.Expression);
            VisitToken(Tokens.T_SEMI, ";");
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
                VisitToken(Tokens.T_TRY, "try");
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
                VisitModifiers(x, x.MemberAttributes);
                if ((x.MemberAttributes & PhpMemberAttributes.Interface) != 0) VisitToken(Tokens.T_INTERFACE, "interface");
                else if ((x.MemberAttributes & PhpMemberAttributes.Trait) != 0) VisitToken(Tokens.T_TRAIT, "trait");
                else VisitToken(Tokens.T_CLASS, "class");

                if (x.Name.HasValue)
                {
                    VisitToken(Tokens.T_STRING, x.Name.Name.Value);
                }

                if (x.BaseClass != null)
                {
                    // extends
                    VisitToken(Tokens.T_EXTENDS, "extends");
                    VisitElement((TypeRef)x.BaseClass);
                }

                if (x.ImplementsList != null && x.ImplementsList.Length != 0)
                {
                    // implements|extends
                    if ((x.MemberAttributes & PhpMemberAttributes.Interface) == 0)
                    {
                        VisitToken(Tokens.T_IMPLEMENTS);
                    }
                    else
                    {
                        VisitToken(Tokens.T_EXTENDS);
                    }

                    VisitElementList(x.ImplementsList, VisitNamedTypeRef, Tokens.T_COMMA, ",");
                }


                VisitToken(Tokens.T_LBRACE, "{");
                VisitList(x.Members);
                VisitToken(Tokens.T_RBRACE, "}");
            }
        }

        public override void VisitTypeOfEx(TypeOfEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnaryEx(UnaryEx x)
        {
            VisitToken(TokenFacts.GetOperationToken(x.Operation));
            VisitElement(x.Expr);
        }

        public override void VisitUnsetStmt(UnsetStmt x)
        {
            VisitToken(Tokens.T_UNSET, "unset");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElementList(x.VarList, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_RPAREN, ")");
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitUseStatement(UseStatement x)
        {
            VisitToken(Tokens.T_USE, "use");
            switch (x.Kind)
            {
                case AliasKind.Constant: VisitToken(Tokens.T_CONST, "const"); break;
                case AliasKind.Function: VisitToken(Tokens.T_FUNCTION, "function"); break;
            }

            VisitElementList(x.Uses, VisitUse, Tokens.T_COMMA, ",");
            VisitToken(Tokens.T_SEMI, ";");
        }

        protected virtual void VisitUse(UseBase use)
        {
            throw new NotImplementedException();
        }

        public override void VisitValueAssignEx(ValueAssignEx x)
        {
            // L = R
            VisitElement(x.LValue);
            VisitToken(Tokens.T_EQ, "=");
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
                VisitToken(Tokens.T_WHILE, "while");
                VisitToken(Tokens.T_LPAREN, "(");
                VisitElement(x.CondExpr);
                VisitToken(Tokens.T_RPAREN, ")");
                VisitElement(x.Body);
            }
        }

        public override void VisitYieldEx(YieldEx x)
        {
            VisitToken(Tokens.T_YIELD, "yield");

            if (x.KeyExpr != null)
            {
                VisitElement(x.KeyExpr);
                VisitToken(Tokens.T_DOUBLE_ARROW);
            }

            VisitElement(x.ValueExpr);
        }

        public override void VisitYieldFromEx(YieldFromEx x)
        {
            VisitToken(Tokens.T_YIELD_FROM);
            VisitElement(x.ValueExpr);
        }

        #endregion
    }
}
