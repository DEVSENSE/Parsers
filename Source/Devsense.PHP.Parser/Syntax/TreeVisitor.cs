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

using Devsense.PHP.Syntax.Ast;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Class visits recursively each AstNode 
    /// </summary>
    public class TreeVisitor
    {
        /// <summary>
        /// Visit language element and all children recursively.
        /// Depth-first search.
        /// </summary>
        /// <param name="element">Any LanguageElement. Can be null.</param>
        public virtual void VisitElement(LangElement element)
        {
            if (element != null)
            {
                element.VisitMe(this);
            }
        }

        /// <summary>
        /// Visit global scope element and all children.
        /// </summary>
        /// <param name="x">GlobalCode.</param>
        public virtual void VisitGlobalCode(GlobalCode x)
        {
            VisitList(x.Statements);
        }

        #region Statements

        public virtual void VisitHaltCompiler(HaltCompiler x)
        {
        }

        /// <summary>
        /// Visit statements and catches.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitTryStmt(TryStmt x)
        {
            // try body
            VisitElement(x.Body);

            // visit catch blocks
            VisitList(x.Catches);

            // visit finally block
            VisitElement(x.FinallyItem);
        }

        /// <summary>
        /// Visit throw expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitThrowStmt(ThrowStmt x)
        {
            VisitElement(x.Expression);
        }

        /// <summary>
        /// Visit namespace statements.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitNamespaceDecl(NamespaceDecl x)
        {
            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit constant declarations.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitGlobalConstDeclList(GlobalConstDeclList x)
        {
            VisitList(x.Constants);
        }

        virtual public void VisitGlobalConstantDecl(GlobalConstantDecl x)
        {
            VisitElement(x.Initializer);
        }

        virtual public void VisitTraitAdaptationBlock(TraitAdaptationBlock x)
        {
            VisitList(x.Adaptations);
        }

        /// <summary>
        /// Visit statements in given Block Statement.
        /// </summary>
        /// <param name="x">Block statement.</param>
        virtual public void VisitBlockStmt(BlockStmt x)
        {
            VisitList(x.Statements);
        }

        /// <summary>
        /// Visit expression in given expression statement.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitExpressionStmt(ExpressionStmt x)
        {
            VisitElement(x.Expression);
        }

        virtual public void VisitEmptyStmt(EmptyStmt x)
        {
            // nothing
        }

        /// <summary>
        /// Visit each VariableUse in unset variable list.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitUnsetStmt(UnsetStmt x)
        {
            VisitList(x.VarList);
        }

        /// <summary>
        /// Visit each SimpleVarUse in global variable list. 
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitGlobalStmt(GlobalStmt x)
        {
            VisitList(x.VarList);
        }

        /// <summary>
        /// Visit each StaticVarDecl in static variable list.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitStaticStmt(StaticStmt x)
        {
            VisitList(x.StVarList);
        }

        /// <summary>
        /// Visits <c>declare</c> statement and its inner statement.
        /// </summary>
        virtual public void VisitDeclareStmt(DeclareStmt x)
        {
            VisitElement(x.Statement);
        }

        /// <summary>
        /// Visists a statement representing a PHPDoc
        /// </summary>
        virtual public void VisitPHPDocStmt(PHPDocStmt x)
        {
            VisitElement(x.PHPDoc);
        }

        /// <summary>
        /// Visit all conditional statements.
        /// See VisitConditionalStmt(ConditionalStmt x).
        /// </summary>
        virtual public void VisitIfStmt(IfStmt x)
        {
            x.Conditions.Foreach(VisitConditionalStmt);
        }

        /// <summary>
        /// Visits condition and statement.
        /// </summary>
        virtual public void VisitConditionalStmt(ConditionalStmt x)
        {
            VisitElement(x.Condition);
            VisitElement(x.Statement);
        }

        /// <summary>
        /// Visit type members.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitTypeDecl(TypeDecl x)
        {
            VisitList(x.Members);
        }

        /// <summary>
        /// Visit named type.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitNamedTypeDecl(NamedTypeDecl x)
        {
            VisitTypeDecl(x);
        }

        /// <summary>
        /// Visit anonymous type.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitAnonymousTypeDecl(AnonymousTypeDecl x)
        {
            VisitTypeDecl(x);
        }

        /// <summary>
        /// Visit method parameters and method body.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitMethodDecl(MethodDecl x)
        {
            // function parameters
            VisitList(x.Signature.FormalParams);

            // function return type
            VisitElement(x.ReturnType);

            // function body
            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit each FieldDecl in the given FieldDeclList.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitFieldDeclList(FieldDeclList x)
        {
            VisitList(x.Fields);
        }

        /// <summary>
        /// Visit FieldDecl initializer expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitFieldDecl(FieldDecl x)
        {
            VisitElement(x.Initializer);
        }

        /// <summary>
        /// Visit each ClassConstantDecl in ConstDeclList.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitConstDeclList(ConstDeclList x)
        {
            VisitList(x.Constants);
        }

        /// <summary>
        /// Visit given constant and constant initializer expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitClassConstantDecl(ClassConstantDecl x)
        {
            VisitConstantDecl(x);
        }

        /// <summary>
        /// Visit constant initializer expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitConstantDecl(ConstantDecl x)
        {
            VisitElement(x.Initializer);
        }

        /// <summary>
        /// Visit function parameters and function body.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitFunctionDecl(FunctionDecl x)
        {
            // function parameters
            VisitList(x.Signature.FormalParams);

            // function return type
            VisitElement(x.ReturnType);

            // function body
            VisitElement(x.Body);
        }

        virtual public void VisitTraitsUse(TraitsUse x)
        {
            // visits adaptation list
            VisitElement(x.TraitAdaptationBlock);
        }

        virtual public void VisitTraitAdaptationPrecedence(TraitsUse.TraitAdaptationPrecedence x)
        {

        }

        virtual public void VisitTraitAdaptationAlias(TraitsUse.TraitAdaptationAlias x)
        {

        }

        /// <summary>
        /// Visit expressions in echo statement.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitEchoStmt(EchoStmt x)
        {
            VisitList(x.Parameters);
        }

        /// <summary>
        /// Visit all elements in the given list.
        /// </summary>
        /// <param name="items">Collection of elements to visit.</param>
        protected void VisitList<T>(IList<T> items) where T : LangElement
        {
            if (items != null)
            {
                items.Foreach(VisitElement);
            }
        }

        #endregion

        #region Switch statement

        /// <summary>
        /// Visit switch value and switch items.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitSwitchStmt(SwitchStmt x)
        {
            VisitElement(x.SwitchValue);
            VisitList(x.SwitchItems);
        }

        /// <summary>
        /// Visit switch-case item.
        /// Case expression and case body.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitCaseItem(CaseItem x)
        {
            VisitElement(x.CaseVal);
            VisitSwitchItem(x);
        }

        /// <summary>
        /// Visit switch-default item.
        /// Visit case body.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitDefaultItem(DefaultItem x)
        {
            VisitSwitchItem(x);
        }

        /// <summary>
        /// Called by derived objects visitor (CaseItem and DefaultItem).
        /// Visit all statements in SwitchItem.
        /// </summary>
        /// <param name="x">SwitchItem, CaseItem or DefaultItem.</param>
        virtual public void VisitSwitchItem(SwitchItem x)
        {
            VisitList(x.Statements);
        }

        #endregion

        #region Jumps statements

        virtual public void VisitJumpStmt(JumpStmt x)
        {
            VisitElement(x.Expression);
        }
        virtual public void VisitGotoStmt(GotoStmt x)
        {
            // x.LabelName
        }
        virtual public void VisitLabelStmt(LabelStmt x)
        {
            // x.Name
        }

        #endregion

        #region  Cycle statements

        /// <summary>
        /// Visit cycle condition expression and cycle body.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitWhileStmt(WhileStmt x)
        {
            VisitElement(x.CondExpr);
            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit "for" initialization,condition and action expressions.
        /// Visit "for" body.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitForStmt(ForStmt x)
        {
            VisitList(x.InitExList);
            VisitList(x.CondExList);
            VisitList(x.ActionExList);

            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit enumeree and body.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitForeachStmt(ForeachStmt x)
        {
            VisitElement(x.Enumeree);
            VisitForeachVar(x.KeyVariable);
            VisitForeachVar(x.ValueVariable);
            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit foreach variable used for value and key.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitForeachVar(ForeachVar x)
        {
            VisitElement(x?.Target);
        }

        #endregion

        #region Expressions

        /*/// <summary>
        /// Called when derived class visited.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitVarLikeConstructUse(VarLikeConstructUse x)
        {
            // base for variable use
        }*/

        /// <summary>
        /// Called when derived class visited.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitConstantUse(ConstantUse x)
        {
            // base for constant use
        }

        virtual public void VisitDirectVarUse(DirectVarUse x)
        {
            VisitVarLikeConstructUse(x);
        }
        virtual public void VisitGlobalConstUse(GlobalConstUse x)
        {
            VisitConstantUse(x);
        }
        virtual public void VisitClassConstUse(ClassConstUse x)
        {
            VisitElement(x.TargetType);
            VisitConstantUse(x);
        }
        virtual public void VisitPseudoClassConstUse(PseudoClassConstUse x)
        {
            VisitClassConstUse(x);
        }
        virtual public void VisitPseudoConstUse(PseudoConstUse x)
        {
            // nothing
        }
        virtual public void VisitIndirectVarUse(IndirectVarUse x)
        {
            VisitVarLikeConstructUse(x);
            VisitElement(x.VarNameEx);
        }

        virtual public void VisitVarLikeConstructUse(VarLikeConstructUse x)
        {
            VisitElement(x.IsMemberOf);
        }

        /// <summary>
        /// Visit include target.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitIncludingEx(IncludingEx x)
        {
            VisitElement(x.Target);
        }

        /// <summary>
        /// Visit each VariableUse in isset variable list.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitIssetEx(IssetEx x)
        {
            VisitList(x.VarList);
        }

        /// <summary>
        /// Visit parameter of "empty".
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitEmptyEx(EmptyEx x)
        {
            VisitElement(x.Expression);
        }

        /// <summary>
        /// Visit parameter of "eval".
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitEvalEx(EvalEx x)
        {
            VisitElement(x.Code);
        }

        /// <summary>
        /// Visit exit expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitExitEx(ExitEx x)
        {
            VisitElement(x.ResulExpr);
        }

        /// <summary>
        /// Visit left and right expressions of binary expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitBinaryEx(BinaryEx x)
        {
            VisitElement(x.LeftExpr);
            VisitElement(x.RightExpr);
        }

        /// <summary>
        /// Visit shell command expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitShellEx(ShellEx x)
        {
            VisitElement(x.Command);
        }

        /// <summary>
        /// Visit item use index (if not null) and array.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitItemUse(ItemUse x)
        {
            VisitVarLikeConstructUse(x);
            VisitElement(x.Index);
            VisitElement(x.Array);
        }

        /// <summary>
        /// Visit item use index (if not null) and array.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitValueItem(ValueItem x)
        {
            VisitElement(x.Index);
            VisitElement(x.ValueExpr);
        }

        /// <summary>
        /// Visit item use index (if not null) and array.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitRefItem(RefItem x)
        {
            VisitElement(x.Index);
            VisitElement(x.RefToGet);
        }

        /// <summary>
        /// Visits string literal dereferencing.
        /// </summary>
        virtual public void VisitStringLiteralDereferenceEx(StringLiteralDereferenceEx x)
        {
            VisitElement(x.StringExpr);
            VisitElement(x.KeyExpr);
        }

        /// <summary>
        /// Called when derived class visited.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitFunctionCall(FunctionCall x)
        {
            VisitVarLikeConstructUse(x);
            VisitList(x.CallSignature.Parameters);
        }

        /// <summary>
        /// Visit function call actual parameters.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitDirectFcnCall(DirectFcnCall x)
        {
            VisitFunctionCall(x);
        }

        /// <summary>
        /// Visit name expression and actual parameters.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitIndirectFcnCall(IndirectFcnCall x)
        {
            VisitElement(x.NameExpr);
            VisitFunctionCall(x);
        }

        /// <summary>
        /// Visit function call actual parameters.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            VisitFunctionCall(x);
        }

        /// <summary>
        /// Visit name expression and method call actual parameters.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            VisitElement(x.TargetType);
            VisitElement(x.MethodNameVar);
            VisitFunctionCall(x);
        }
        virtual public void VisitDirectStFldUse(DirectStFldUse x)
        {
            VisitElement(x.TargetType);
        }
        virtual public void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            VisitElement(x.FieldNameExpr);
            VisitElement(x.TargetType);
        }

        /// <summary>
        /// Visit new array items initializers.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitArrayEx(ArrayEx x)
        {
            x.Items.Foreach(VisitArrayItem);
        }

        virtual public void VisitArrayItem(Item item)
        {
            if (item != null)   // list() may have 'null' items
            {
                // key
                VisitElement(item.Index);

                // value
                if (item is ValueItem)
                    VisitElement(((ValueItem)item).ValueExpr);
                else
                    VisitElement(((RefItem)item).RefToGet);
            }
        }

        /// <summary>
        /// Visit conditions.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitConditionalEx(ConditionalEx x)
        {
            VisitElement(x.CondExpr);
            VisitElement(x.TrueExpr);
            VisitElement(x.FalseExpr);
        }

        /// <summary>
        /// Visit variable that is incremented (or decremented).
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitIncDecEx(IncDecEx x)
        {
            VisitElement(x.Variable);
        }

        /// <summary>
        /// Visit l-value of assignment.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitAssignEx(AssignEx x)
        {
            VisitElement(x.LValue);
        }

        /// <summary>
        /// Visit left and right values in assignment.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitValueAssignEx(ValueAssignEx x)
        {
            VisitAssignEx(x);
            VisitElement(x.RValue);
        }

        /// <summary>
        /// Visit left and right values in ref assignment.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitRefAssignEx(RefAssignEx x)
        {
            VisitAssignEx(x);
            VisitElement(x.RValue);
        }

        /// <summary>
        /// Visit unary expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitUnaryEx(UnaryEx x)
        {
            VisitElement(x.Expr);
        }

        /// <summary>
        /// Visit "new" call parameters.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitNewEx(NewEx x)
        {
            VisitElement(x.ClassNameRef);
            VisitList(x.CallSignature.Parameters);
        }

        /// <summary>
        /// Visit instanceof expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitInstanceOfEx(InstanceOfEx x)
        {
            VisitElement(x.Expression);
            VisitElement(x.ClassNameRef);
        }

        /// <summary>
        /// Visit typeof ClassNameRef expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitTypeOfEx(TypeOfEx x)
        {
            VisitElement(x.ClassNameRef);
        }

        /// <summary>
        /// Visit expressions in PHP concat.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitConcatEx(ConcatEx x)
        {
            VisitList(x.Expressions);
        }

        /// <summary>
        /// Visit list initializer expressions and r-value (if not null)
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitListEx(ListEx x)
        {
            x.Items.Foreach(VisitArrayItem);
        }

        /// <summary>
        /// Visit <see cref="LambdaFunctionExpr"/> expression.
        /// </summary>
        virtual public void VisitLambdaFunctionExpr(LambdaFunctionExpr x)
        {
            // function parameters
            VisitList(x.Signature.FormalParams);

            // use parameters
            VisitList(x.UseParams);

            // function return type
            VisitElement(x.ReturnType);

            // function body
            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit <see cref="YieldEx"/> expression.
        /// </summary>
        virtual public void VisitYieldEx(YieldEx x)
        {
            VisitElement(x.KeyExpr);
            VisitElement(x.ValueExpr);
        }

        /// <summary>
        /// Visit <see cref="YieldFromEx"/> expression.
        /// </summary>
        virtual public void VisitYieldFromEx(YieldFromEx x)
        {
            VisitElement(x.ValueExpr);
        }
        #endregion

        #region Literals

        virtual public void VisitLongIntLiteral(LongIntLiteral x)
        {
            // nothing
        }

        virtual public void VisitDoubleLiteral(DoubleLiteral x)
        {
            // nothing
        }

        virtual public void VisitStringLiteral(StringLiteral x)
        {
            // nothing
        }

        virtual public void VisitBinaryStringLiteral(BinaryStringLiteral x)
        {
            // nothing
        }

        virtual public void VisitBoolLiteral(BoolLiteral x)
        {
            // nothing
        }

        virtual public void VisitNullLiteral(NullLiteral x)
        {
            // nothing
        }

        #endregion

        #region Others

        /// <summary>
        /// Visit catch. Variable first then body statements.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitCatchItem(CatchItem x)
        {
            VisitElement(x.TypeRef);
            VisitElement(x.Variable);
            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit <see cref="FinallyItem"/>.
        /// </summary>
        virtual public void VisitFinallyItem(FinallyItem x)
        {
            VisitElement(x.Body);
        }

        /// <summary>
        /// Visit static variable declaration, variable name and initializer expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitStaticVarDecl(StaticVarDecl x)
        {
            VisitElement(x.Initializer);
        }

        virtual public void VisitFormalTypeParam(FormalTypeParam x)
        {
            // nothing
        }

        /// <summary>
        /// Visit custom attributes NamedParameters.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitCustomAttribute(CustomAttribute x)
        {
            VisitList(x.NamedParameters);
        }

        /// <summary>
        /// Visit formal parameter initializer expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitFormalParam(FormalParam x)
        {
            VisitElement(x.TypeHint);
            VisitElement(x.InitValue);
        }

        /// <summary>
        /// Visit actual parameter expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitActualParam(ActualParam x)
        {
            VisitElement(x.Expression);
        }

        /// <summary>
        /// Visit named actual parameter expression.
        /// </summary>
        /// <param name="x"></param>
        virtual public void VisitNamedActualParam(NamedActualParam x)
        {
            VisitElement(x.Expression);
        }

        virtual public void VisitPrimitiveTypeRef(PrimitiveTypeRef x)
        {
            // nothing
        }
        virtual public void VisitDirectTypeRef(DirectTypeRef x)
        {
            // nothing
        }
        virtual public void VisitIndirectTypeRef(IndirectTypeRef x)
        {
            VisitElement(x.ClassNameVar);
        }
        virtual public void VisitNullableTypeRef(NullableTypeRef x)
        {
            VisitElement(x.TargetType);
        }
        virtual public void VisitMultipleTypeRef(MultipleTypeRef x)
        {
            VisitList(x.MultipleTypes);
        }
        virtual public void VisitGenericTypeRef(GenericTypeRef x)
        {
            VisitElement(x.TargetType);
        }
        virtual public void VisitAnonymousTypeRef(AnonymousTypeRef x)
        {
            VisitElement(x.TypeDeclaration);
        }
        virtual public void VisitAssertEx(AssertEx x)
        {
            VisitElement(x.CodeEx);
            // TODO: x.DescriptionEx
        }

        virtual public void VisitPHPDocBlock(PHPDocBlock x)
        {

        }

        #endregion
    }
}