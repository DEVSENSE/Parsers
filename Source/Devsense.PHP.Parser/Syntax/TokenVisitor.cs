using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devsense.PHP.Syntax.Ast;

namespace Devsense.PHP.Syntax
{
    public class TokenVisitor : TreeContextVisitor
    {
        public TokenVisitor(TreeContext initialContext) : base(initialContext)
        {
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

        #region Single Nodes Overrides

        public override void VisitElement(LangElement element)
        {
            base.VisitElement(element);
        }

        public override void VisitActualParam(ActualParam x)
        {
            throw new NotImplementedException();
        }

        public override void VisitAnonymousTypeDecl(AnonymousTypeDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitAnonymousTypeRef(AnonymousTypeRef x)
        {
            base.VisitAnonymousTypeRef(x);
        }

        public override void VisitArrayEx(ArrayEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitArrayItem(Item item)
        {
            throw new NotImplementedException();
        }

        public override void VisitAssertEx(AssertEx x)
        {
            VisitToken(Tokens.T_STRING, "assert");
        }

        public sealed override void VisitAssignEx(AssignEx x) { throw new InvalidOperationException(); }

        public override void VisitBinaryEx(BinaryEx x)
        {
            throw new NotImplementedException();
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
            VisitToken(Tokens.T_STRING, x.Value.ToString().ToLowerInvariant());
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
            VisitToken(Tokens.T_CATCH, "catch");
            VisitToken(Tokens.T_LPAREN, "(");
            VisitElement(x.TargetType);
            VisitToken(Tokens.T_RPAREN, ")");
            using (new ScopeHelper(this, x))
            {
                VisitElement(x.Body);
            }
        }

        public override void VisitClassConstantDecl(ClassConstantDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitClassConstUse(ClassConstUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitClassTypeRef(ClassTypeRef x)
        {
            throw new NotImplementedException();
        }

        public override void VisitConcatEx(ConcatEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitConditionalEx(ConditionalEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitConditionalStmt(ConditionalStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstantDecl(ConstantDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstantUse(ConstantUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitConstDeclList(ConstDeclList x)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void VisitDirectFcnCall(DirectFcnCall x)
        {
            throw new NotImplementedException();
        }

        public override void VisitDirectStFldUse(DirectStFldUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            throw new NotImplementedException();
        }

        public override void VisitDirectVarUse(DirectVarUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitDoubleLiteral(DoubleLiteral x)
        {
            throw new NotImplementedException();
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
                for (int i = 0; i < x.Parameters.Length; i++)
                {
                    if (i != 0) VisitToken(Tokens.T_COMMA, ",");
                    VisitElement(x.Parameters[i]);
                }
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
            throw new NotImplementedException();
        }

        public override void VisitFieldDeclList(FieldDeclList x)
        {
            throw new NotImplementedException();
        }

        public override void VisitFinallyItem(FinallyItem x)
        {
            // finally BLOCK
            VisitToken(Tokens.T_FINAL, "finally");
            base.VisitFinallyItem(x);
        }

        public override void VisitForeachStmt(ForeachStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitForeachVar(ForeachVar x)
        {
            throw new NotImplementedException();
        }

        public override void VisitFormalParam(FormalParam x)
        {
            throw new NotImplementedException();
        }

        public override void VisitFormalTypeParam(FormalTypeParam x)
        {
            throw new NotImplementedException();
        }

        public override void VisitForStmt(ForStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitFunctionCall(FunctionCall x)
        {
            throw new NotImplementedException();
        }

        public override void VisitFunctionDecl(FunctionDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitGenericTypeRef(GenericTypeRef x)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobalCode(GlobalCode x)
        {
            base.VisitGlobalCode(x);
        }

        public override void VisitGlobalConstantDecl(GlobalConstantDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobalConstDeclList(GlobalConstDeclList x)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobalConstUse(GlobalConstUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobalStmt(GlobalStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitGotoStmt(GotoStmt x)
        {
            VisitToken(Tokens.T_GOTO, "goto");
            VisitToken(Tokens.T_STRING, x.LabelName.Name.Value);
            VisitToken(Tokens.T_SEMI, ";");
        }

        public override void VisitHaltCompiler(HaltCompiler x)
        {
            throw new NotImplementedException();
        }

        public override void VisitIfStmt(IfStmt x)
        {
            throw new NotImplementedException();
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
                    throw new NotImplementedException();// ??
            }

            VisitElement(x.Target);
        }

        public override void VisitIndirectFcnCall(IndirectFcnCall x)
        {
            if (x.IsMemberOf != null)
            {
                VisitElement(x.IsMemberOf);
                VisitToken(Tokens.T_OBJECT_OPERATOR, "->");
            }

            VisitElement(x.NameExpr);
            VisitCallSignature(x.CallSignature);
        }

        public virtual void VisitCallSignature(CallSignature signature)
        {
            VisitToken(Tokens.T_LPAREN, "(");
            for (int i = 0; i < signature.Parameters.Length; i++)
            {
                if (i != 0) VisitToken(Tokens.T_COMMA, ",");
                VisitElement(signature.Parameters[i]);
            }
            VisitToken(Tokens.T_RPAREN, ")");
        }

        public override void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            throw new NotImplementedException();
        }

        public override void VisitIndirectTypeRef(IndirectTypeRef x)
        {
            throw new NotImplementedException();
        }

        public override void VisitIndirectVarUse(IndirectVarUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitInstanceOfEx(InstanceOfEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitIssetEx(IssetEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitItemUse(ItemUse x)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void VisitListEx(ListEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitLongIntLiteral(LongIntLiteral x)
        {
            //VisitToken(Tokens.T_LNUMBER, x.Value, ...)
            throw new NotImplementedException();
        }

        public override void VisitMethodDecl(MethodDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitMultipleTypeRef(MultipleTypeRef x)
        {
            throw new NotImplementedException();
        }

        public override void VisitNamedActualParam(NamedActualParam x)
        {
            throw new NotImplementedException();
        }

        public override void VisitNamedTypeDecl(NamedTypeDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitNamespaceDecl(NamespaceDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitNewEx(NewEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitNullableTypeRef(NullableTypeRef x)
        {
            VisitToken(Tokens.T_QUESTION, "?");
            VisitElement(x.TargetType);
        }

        public override void VisitNullLiteral(NullLiteral x)
        {
            VisitToken(Tokens.T_STRING, "null");
        }

        public override void VisitPHPDocBlock(PHPDocBlock x)
        {
            throw new NotImplementedException();
        }

        public override void VisitPHPDocStmt(PHPDocStmt x)
        {
            base.VisitPHPDocStmt(x);
        }

        public override void VisitPrimitiveTypeRef(PrimitiveTypeRef x)
        {
            throw new NotImplementedException();
        }

        public override void VisitPseudoClassConstUse(PseudoClassConstUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitPseudoConstUse(PseudoConstUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitRefAssignEx(RefAssignEx x)
        {
            // L =& R
            VisitElement(x.LValue);
            VisitToken(Tokens.T_EQ, "=");
            VisitToken(Tokens.T_AMP, "&");
            VisitElement(x.RValue);
        }

        public override void VisitRefItem(RefItem x)
        {
            base.VisitRefItem(x);
        }

        public override void VisitReservedTypeRef(ReservedTypeRef x)
        {
            throw new NotImplementedException();
        }

        public override void VisitShellEx(ShellEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitStaticStmt(StaticStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitStaticVarDecl(StaticVarDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitStringLiteral(StringLiteral x)
        {
            throw new NotImplementedException();
        }

        public override void VisitStringLiteralDereferenceEx(StringLiteralDereferenceEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitSwitchItem(SwitchItem x)
        {
            throw new NotImplementedException();
        }

        public override void VisitSwitchStmt(SwitchStmt x)
        {
            // switch(VALUE){CASES}
            VisitToken(Tokens.T_SWITCH, "switch");
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
            throw new NotImplementedException();
        }

        public override void VisitTryStmt(TryStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeDecl(TypeDecl x)
        {
            throw new NotImplementedException();
        }

        public override void VisitTypeOfEx(TypeOfEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnaryEx(UnaryEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUnsetStmt(UnsetStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitUseStatement(UseStatement x)
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

        public override void VisitValueItem(ValueItem x)
        {
            throw new NotImplementedException();
        }

        public override void VisitVarLikeConstructUse(VarLikeConstructUse x)
        {
            throw new NotImplementedException();
        }

        public override void VisitWhileStmt(WhileStmt x)
        {
            throw new NotImplementedException();
        }

        public override void VisitYieldEx(YieldEx x)
        {
            throw new NotImplementedException();
        }

        public override void VisitYieldFromEx(YieldFromEx x)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
