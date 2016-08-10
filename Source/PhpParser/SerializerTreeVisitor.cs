using PHP.Core.AST;
using PHP.Core.Text;
using PHP.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using property = System.Tuple<string, string>;

namespace PhpParser
{
    public class SerializerTreeVisitor : TreeVisitor
    {
        ISerializer _serializer;

        public SerializerTreeVisitor(ISerializer serializer) : base()
        {
            _serializer = serializer;
        }

        string MemberAttributesToString(PhpMemberAttributes attr)
        {
            //if (attr == PhpMemberAttributes.None)
            //    return "None";
            switch (attr)
            {
                case PhpMemberAttributes.Public:
                    return "Public";
                case PhpMemberAttributes.Private:
                    return "Private";
                case PhpMemberAttributes.Protected:
                    return "Protected";
                case PhpMemberAttributes.Static:
                    return "Static";
                case PhpMemberAttributes.AppStatic:
                    return "AppStatic";
                case PhpMemberAttributes.Abstract:
                    return "Abstract";
                case PhpMemberAttributes.Final:
                    return "Final";
                case PhpMemberAttributes.Interface:
                    return "Interface";
                case PhpMemberAttributes.Trait:
                    return "Trait";
                case PhpMemberAttributes.Constructor:
                    return "Constructor";
                case PhpMemberAttributes.Ambiguous:
                    return "Ambiguous";
                case PhpMemberAttributes.InactiveConditional:
                    return "InactiveConditional";
                default:
                    return "Error";
            }
        }

        NodeObj SerializeSpan(Span span)
        {
            return new NodeObj("Span", new NodeObj("start", span.Start.ToString()), new NodeObj("end", span.End.ToString()));
        }

        NodeObj SerializeNamingContext(NamingContext context)
        {
            List<NodeObj> data = new List<NodeObj>();
            if (context.CurrentNamespace.HasValue)
                data.Add(new NodeObj("Namespace", context.CurrentNamespace.Value.NamespacePhpName));
            if (context.Aliases != null && context.Aliases.Count > 0)
                data.Add(new NodeObj("Aliases", context.Aliases.Select(a => new NodeObj(a.Key, a.Value.ToString())).ToArray()));
            if (context.ConstantAliases != null && context.ConstantAliases.Count > 0)
                data.Add(new NodeObj("ConstantAliases", context.ConstantAliases.Select(a => new NodeObj(a.Key, a.Value.ToString())).ToArray()));
            if (context.FunctionAliases != null && context.FunctionAliases.Count > 0)
                data.Add(new NodeObj("FunctionAliases", context.FunctionAliases.Select(a => new NodeObj(a.Key, a.Value.ToString())).ToArray()));
            return new NodeObj("NamingContext", data.ToArray());
        }

        #region Literals

        override public void VisitLongIntLiteral(LongIntLiteral x)
        {
            _serializer.Serialize(typeof(LongIntLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", x.Value.ToString()));
        }

        override public void VisitDoubleLiteral(DoubleLiteral x)
        {
            _serializer.Serialize(typeof(DoubleLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", x.Value.ToString()));
        }

        override public void VisitStringLiteral(StringLiteral x)
        {
            _serializer.Serialize(typeof(StringLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", x.Value));
        }

        override public void VisitBinaryStringLiteral(BinaryStringLiteral x)
        {
            _serializer.Serialize(typeof(BinaryStringLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", x.Value.ToString()));
        }

        override public void VisitBoolLiteral(BoolLiteral x)
        {
            _serializer.Serialize(typeof(BoolLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", x.Value.ToString()));
        }

        override public void VisitNullLiteral(NullLiteral x)
        {
            _serializer.Serialize(typeof(NullLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", "null"));
        }

        #endregion

        /// <summary>
        /// Visit expressions in echo statement.
        /// </summary>
        /// <param name="x"></param>
        override public void VisitEchoStmt(EchoStmt x)
        {
            _serializer.StartSerialize(typeof(EchoStmt).Name, SerializeSpan(x.Span));
            base.VisitEchoStmt(x);
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit global scope element and all children.
        /// </summary>
        /// <param name="x">GlobalCode.</param>
        override public void VisitGlobalCode(GlobalCode x)
        {
            _serializer.StartSerialize(typeof(GlobalCode).Name, SerializeSpan(x.Span), SerializeNamingContext(x.SourceUnit.Naming));
            base.VisitGlobalCode(x);
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit namespace statements.
        /// </summary>
        /// <param name="x"></param>
        override public void VisitNamespaceDecl(NamespaceDecl x)
        {
            if (string.IsNullOrEmpty(x.QualifiedName.NamespacePhpName))
                _serializer.StartSerialize(typeof(NamespaceDecl).Name, SerializeSpan(x.Span),
                    new NodeObj("SimpleSyntax", x.IsSimpleSyntax.ToString()),
                    SerializeNamingContext(x.Naming));
            else
                _serializer.StartSerialize(typeof(NamespaceDecl).Name, SerializeSpan(x.Span),
                    new NodeObj("Name", x.QualifiedName.NamespacePhpName),
                    new NodeObj("SimpleSyntax", x.IsSimpleSyntax.ToString()),
                    SerializeNamingContext(x.Naming));
            base.VisitNamespaceDecl(x);
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit constant declarations.
        /// </summary>
        /// <param name="x"></param>
        override public void VisitGlobalConstDeclList(GlobalConstDeclList x)
        {
            _serializer.StartSerialize(typeof(GlobalConstDeclList).Name, SerializeSpan(x.Span));
            base.VisitGlobalConstDeclList(x);
            _serializer.EndSerialize();
        }

        override public void VisitGlobalConstantDecl(GlobalConstantDecl x)
        {
            _serializer.StartSerialize(typeof(GlobalConstantDecl).Name, SerializeSpan(x.Span),
                new NodeObj("NameIsConditional", x.IsConditional.ToString()), new NodeObj("Name", x.Name.Value));
            if (x.PHPDoc != null)
                _serializer.Serialize("PHPDoc", new NodeObj("Comment", x.PHPDoc.ToString()));
            VisitElement(x.Initializer);
            _serializer.EndSerialize();
        }

        override public void VisitIncDecEx(IncDecEx x)
        {
            _serializer.StartSerialize(typeof(IncDecEx).Name, SerializeSpan(x.Span),
                new NodeObj("Inc", x.Inc.ToString()), new NodeObj("Post", x.Post.ToString()));
            base.VisitIncDecEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitAssignEx(AssignEx x)
        {
            base.VisitAssignEx(x);
        }

        override public void VisitValueAssignEx(ValueAssignEx x)
        {
            _serializer.StartSerialize(typeof(ValueAssignEx).Name, SerializeSpan(x.Span),
                new NodeObj("Operation", x.Operation.ToString()));
            base.VisitValueAssignEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitRefAssignEx(RefAssignEx x)
        {
            _serializer.StartSerialize(typeof(RefAssignEx).Name, SerializeSpan(x.Span),
                new NodeObj("Operation", x.Operation.ToString()));
            base.VisitRefAssignEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitUnaryEx(UnaryEx x)
        {
            _serializer.StartSerialize(typeof(UnaryEx).Name, SerializeSpan(x.Span),
                new NodeObj("Operation", x.Operation.ToString()));
            base.VisitUnaryEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitBinaryEx(BinaryEx x)
        {
            _serializer.StartSerialize(typeof(BinaryEx).Name, SerializeSpan(x.Span),
                new NodeObj("Operation", x.Operation.ToString()));
            base.VisitBinaryEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitDirectFcnCall(DirectFcnCall x)
        {
            _serializer.StartSerialize(typeof(DirectFcnCall).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.QualifiedName.ToString()));
            if (x.IsMemberOf != null)
            {
                _serializer.StartSerialize("IsMemberOf");
                VisitMemberOf(x);
                _serializer.EndSerialize();
            }

            foreach (ActualParam p in x.CallSignature.Parameters)
                VisitElement(p);
            _serializer.EndSerialize();
        }
        override public void VisitIndirectFcnCall(IndirectFcnCall x)
        {
            _serializer.StartSerialize(typeof(IndirectFcnCall).Name, SerializeSpan(x.Span));
            VisitElement(x.NameExpr);
            if (x.IsMemberOf != null)
            {
                _serializer.StartSerialize("IsMemberOf");
                VisitMemberOf(x);
                _serializer.EndSerialize();
            }

            foreach (ActualParam p in x.CallSignature.Parameters)
                VisitElement(p);
            _serializer.EndSerialize();
        }
        void VisitMemberOf(VarLikeConstructUse x)
        {
            if (x is DirectVarUse)
                VisitDirectVarUse((DirectVarUse)x);
            else if (x is IndirectVarUse)
                VisitIndirectVarUse((IndirectVarUse)x);
            if (x.IsMemberOf != null)
                VisitMemberOf(x.IsMemberOf);
        }
        override public void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            _serializer.StartSerialize(typeof(DirectStMtdCall).Name, SerializeSpan(x.Span),
                new NodeObj("ClassName", x.ClassName.QualifiedName.ToString()),
                new NodeObj("MethodName", x.MethodName.Value));
            base.VisitDirectStMtdCall(x);
            _serializer.EndSerialize();
        }
        override public void VisitIndirectStMtdCall(IndirectStMtdCall x)
        {
            _serializer.StartSerialize(typeof(IndirectStMtdCall).Name, SerializeSpan(x.Span));
            base.VisitIndirectStMtdCall(x);
            _serializer.EndSerialize();
        }
        override public void VisitActualParam(ActualParam x)
        {
            _serializer.StartSerialize(typeof(ActualParam).Name, SerializeSpan(x.Span),
                new NodeObj("IsUnpack", x.IsUnpack.ToString()));
            base.VisitActualParam(x);
            _serializer.EndSerialize();
        }
        override public void VisitBlockStmt(BlockStmt x)
        {
            _serializer.StartSerialize(typeof(BlockStmt).Name, SerializeSpan(x.Span));
            base.VisitBlockStmt(x);
            _serializer.EndSerialize();
        }

        override public void VisitJumpStmt(JumpStmt x)
        {
            _serializer.StartSerialize(typeof(JumpStmt).Name, SerializeSpan(x.Span),
                new NodeObj("Type", x.Type.ToString()));
            base.VisitJumpStmt(x);
            _serializer.EndSerialize();
        }
        override public void VisitWhileStmt(WhileStmt x)
        {
            _serializer.StartSerialize(typeof(WhileStmt).Name, SerializeSpan(x.Span),
                new NodeObj("LoopType", x.LoopType.ToString()));
            _serializer.StartSerialize("CondExpr");
            VisitElement(x.CondExpr);
            _serializer.EndSerialize();
            _serializer.StartSerialize("Body");
            VisitElement(x.Body);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }

        override public void VisitDirectVarUse(DirectVarUse x)
        {
            _serializer.StartSerialize(typeof(DirectVarUse).Name, SerializeSpan(x.Span),
                new NodeObj("VarName", x.VarName.Value));
            base.VisitDirectVarUse(x);
            _serializer.EndSerialize();
        }
        override public void VisitIndirectVarUse(IndirectVarUse x)
        {
            _serializer.StartSerialize(typeof(IndirectVarUse).Name, SerializeSpan(x.Span));
            base.VisitIndirectVarUse(x);
            _serializer.EndSerialize();
        }
        override public void VisitConcatEx(ConcatEx x)
        {
            _serializer.StartSerialize(typeof(ConcatEx).Name, SerializeSpan(x.Span));
            base.VisitConcatEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitConditionalEx(ConditionalEx x)
        {
            _serializer.StartSerialize(typeof(ConditionalEx).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("CondExpr");
            VisitElement(x.CondExpr);
            _serializer.EndSerialize();
            _serializer.StartSerialize("TrueExpr");
            VisitElement(x.TrueExpr);
            _serializer.EndSerialize();
            _serializer.StartSerialize("FalseExpr");
            VisitElement(x.FalseExpr);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitForStmt(ForStmt x)
        {
            _serializer.StartSerialize(typeof(WhileStmt).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("InitExList");
            VisitExpressions(x.InitExList);
            _serializer.EndSerialize();
            _serializer.StartSerialize("CondExList");
            VisitExpressions(x.CondExList);
            _serializer.EndSerialize();
            _serializer.StartSerialize("ActionExList");
            VisitExpressions(x.ActionExList);
            _serializer.EndSerialize();
            _serializer.StartSerialize("Body");
            VisitElement(x.Body);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitIfStmt(IfStmt x)
        {
            _serializer.StartSerialize(typeof(IfStmt).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("CondList");
            base.VisitIfStmt(x);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitConditionalStmt(ConditionalStmt x)
        {
            _serializer.StartSerialize(typeof(ConditionalStmt).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("Condition");
            if (x.Condition != null)
                VisitElement(x.Condition);
            _serializer.EndSerialize();

            _serializer.StartSerialize("Statement");
            VisitElement(x.Statement);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitForeachStmt(ForeachStmt x)
        {
            _serializer.StartSerialize(typeof(ForeachStmt).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("Enumeree");
            VisitElement(x.Enumeree);
            _serializer.EndSerialize();
            _serializer.StartSerialize("KeyVariable");
            if (x.KeyVariable != null)
                VisitElement(x.KeyVariable.Variable);
            _serializer.EndSerialize();
            _serializer.StartSerialize("Body");
            VisitElement(x.Body);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitSwitchStmt(SwitchStmt x)
        {
            _serializer.StartSerialize(typeof(SwitchStmt).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("SwitchValue");
            VisitElement(x.SwitchValue);
            _serializer.EndSerialize();
            _serializer.StartSerialize("SwitchItems");
            foreach (SwitchItem item in x.SwitchItems)
                VisitElement(item);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }

        override public void VisitCaseItem(CaseItem x)
        {
            _serializer.StartSerialize(typeof(CaseItem).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("CaseVal");
            VisitElement(x.CaseVal);
            _serializer.EndSerialize();
            _serializer.StartSerialize("Statements");
            VisitSwitchItem(x);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }

        override public void VisitDefaultItem(DefaultItem x)
        {
            _serializer.StartSerialize(typeof(DefaultItem).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("Statements");
            VisitSwitchItem(x);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitShellEx(ShellEx x)
        {
            _serializer.StartSerialize(typeof(ShellEx).Name, SerializeSpan(x.Span));
            base.VisitShellEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitTryStmt(TryStmt x)
        {
            _serializer.StartSerialize(typeof(TryStmt).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("Statements");
            // visit statements
            VisitStatements(x.Statements);
            _serializer.EndSerialize();

            _serializer.StartSerialize("Catches");
            // visit catch blocks
            if (x.Catches != null)
                foreach (CatchItem c in x.Catches)
                    VisitElement(c);
            _serializer.EndSerialize();

            _serializer.StartSerialize("FinallyItem");
            // visit finally block
            VisitElement(x.FinallyItem);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitCatchItem(CatchItem x)
        {
            _serializer.StartSerialize(typeof(CatchItem).Name, SerializeSpan(x.Span));
            _serializer.StartSerialize("TypeRef");
            foreach (var type in x.TypeRef)
                VisitElement(type);
            _serializer.EndSerialize();
            _serializer.StartSerialize("Variable");
            VisitElement(x.Variable);
            _serializer.EndSerialize();
            _serializer.StartSerialize("Statements");
            VisitStatements(x.Statements);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitDirectTypeRef(DirectTypeRef x)
        {
            _serializer.StartSerialize(typeof(DirectTypeRef).Name, SerializeSpan(x.Span), 
                new NodeObj("ClassName", x.ClassName.ToString()), 
                new NodeObj("IsNullable", x.IsNullable.ToString()));
            _serializer.StartSerialize("GenericParams");
            foreach (var param in x.GenericParams)
                VisitElement(param);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitIndirectTypeRef(IndirectTypeRef x)
        {
            _serializer.StartSerialize(typeof(IndirectTypeRef).Name, SerializeSpan(x.Span));
            base.VisitIndirectTypeRef(x);
            _serializer.EndSerialize();
        }
        override public void VisitGotoStmt(GotoStmt x)
        {
            _serializer.Serialize(typeof(GotoStmt).Name, SerializeSpan(x.Span), new NodeObj("LabelName", x.LabelName.Value));
        }
        override public void VisitLabelStmt(LabelStmt x)
        {
            _serializer.Serialize(typeof(LabelStmt).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Value));
        }
        override public void VisitNewEx(NewEx x)
        {
            _serializer.StartSerialize(typeof(NewEx).Name, SerializeSpan(x.Span));
            base.VisitNewEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitFunctionDecl(FunctionDecl x)
        {
            _serializer.StartSerialize(typeof(FunctionDecl).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Value), new NodeObj("IsConditional", x.IsConditional.ToString()));
            if (x.PHPDoc != null)
                _serializer.Serialize("PHPDoc", new NodeObj("Comment", x.PHPDoc.ToString()));
            _serializer.StartSerialize("FormalParams");
            // function parameters
            foreach (FormalParam p in x.Signature.FormalParams)
                VisitElement(p);
            _serializer.EndSerialize();

            _serializer.StartSerialize("Body");
            // function body
            VisitStatements(x.Body);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit <see cref="LambdaFunctionExpr"/> expression.
        /// </summary>
        override public void VisitLambdaFunctionExpr(LambdaFunctionExpr x)
        {
            _serializer.StartSerialize(typeof(LambdaFunctionExpr).Name, SerializeSpan(x.Span));
            if (x.PHPDoc != null)
                _serializer.Serialize("PHPDoc", new NodeObj("Comment", x.PHPDoc.ToString()));
            _serializer.StartSerialize("UseParams");
            // use parameters
            if (x.UseParams != null)
                foreach (var p in x.UseParams)
                    VisitElement(p);
            _serializer.EndSerialize();

            _serializer.StartSerialize("FormalParams");
            // function parameters
            foreach (var p in x.Signature.FormalParams)
                VisitElement(p);
            _serializer.EndSerialize();

            _serializer.StartSerialize("Body");
            // function body
            VisitStatements(x.Body);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitFormalParam(FormalParam x)
        {
            _serializer.StartSerialize(typeof(FormalParam).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Value), 
                new NodeObj("PassedByRef", x.PassedByRef.ToString()), new NodeObj("IsVariadic", x.IsVariadic.ToString()));
            _serializer.StartSerialize("InitValue");
            if (x.InitValue != null)
                VisitElement(x.InitValue);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitYieldEx(YieldEx x)
        {
            _serializer.StartSerialize(typeof(YieldEx).Name, SerializeSpan(x.Span));
            base.VisitYieldEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitDeclareStmt(DeclareStmt x)
        {
            _serializer.StartSerialize(typeof(DeclareStmt).Name, SerializeSpan(x.Span));
            base.VisitDeclareStmt(x);
            _serializer.EndSerialize();
        }
        override public void VisitTypeDecl(TypeDecl x)
        {
            _serializer.StartSerialize(typeof(TypeDecl).Name, SerializeSpan(x.Span), 
                new NodeObj("Name", x.Name.Value), new NodeObj("MemberAttributes", MemberAttributesToString(x.MemberAttributes)),
                new NodeObj("IsConditional", x.IsConditional.ToString()));
            if (x.BaseClassName != null)
                _serializer.Serialize("BaseClassName", new NodeObj("Name", x.BaseClassName.Value.QualifiedName.ToString()));
            if (x.ImplementsList != null && x.ImplementsList.Length > 0)
                _serializer.Serialize("ImplementsList", x.ImplementsList.Select(n => new NodeObj("Name", n.QualifiedName.ToString())).ToArray());
            if (x.PHPDoc != null)
                _serializer.Serialize("PHPDoc", new NodeObj("Comment", x.PHPDoc.ToString()));
            base.VisitTypeDecl(x);
            _serializer.EndSerialize();
        }
        override public void VisitFieldDeclList(FieldDeclList x)
        {
            foreach (FieldDecl f in x.Fields)
                VisitFieldDecl(f, x.Modifiers);
        }
        public void VisitFieldDecl(FieldDecl x, PhpMemberAttributes attributes)
        {
            _serializer.StartSerialize(typeof(FieldDecl).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.Name.Value), new NodeObj("MemberAttributes", MemberAttributesToString(attributes)));
            if (x.PHPDoc != null)
                _serializer.Serialize("PHPDoc", new NodeObj("Comment", x.PHPDoc.ToString()));
            VisitElement(x.Initializer);
            _serializer.EndSerialize();
        }
        override public void VisitConstDeclList(ConstDeclList x)
        {
            foreach (ClassConstantDecl c in x.Constants)
                VisitClassConstantDecl(c, x.Modifiers);
        }
        public void VisitClassConstantDecl(ClassConstantDecl x, PhpMemberAttributes attributes)
        {
            _serializer.StartSerialize(typeof(ClassConstantDecl).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.Name.Value), new NodeObj("MemberAttributes", MemberAttributesToString(attributes)));
            if (x.PHPDoc != null)
                _serializer.Serialize("PHPDoc", new NodeObj("Comment", x.PHPDoc.ToString()));
            VisitElement(x.Initializer);
            _serializer.EndSerialize();
        }
        override public void VisitMethodDecl(MethodDecl x)
        {
            _serializer.StartSerialize(typeof(MethodDecl).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Value), 
                new NodeObj("Modifiers", MemberAttributesToString(x.Modifiers)));
            if (x.PHPDoc != null)
                _serializer.Serialize("PHPDoc", new NodeObj("Comment", x.PHPDoc.ToString()));
            _serializer.StartSerialize("FormalParams");
            // method parameters
            foreach (FormalParam p in x.Signature.FormalParams)
                VisitElement(p);
            _serializer.EndSerialize();

            _serializer.StartSerialize("Body");
            // method body
            VisitStatements(x.Body);
            _serializer.EndSerialize();
            _serializer.EndSerialize();
        }
        override public void VisitUnsetStmt(UnsetStmt x)
        {
            _serializer.StartSerialize(typeof(UnsetStmt).Name, SerializeSpan(x.Span));
            base.VisitUnsetStmt(x);
            _serializer.EndSerialize();
        }

        override public void VisitTraitsUse(TraitsUse x)
        {
            _serializer.StartSerialize(typeof(TraitsUse).Name, SerializeSpan(x.Span));
            _serializer.Serialize("Traits", x.TraitsList.Select(t => new NodeObj("Trait", t.ToString())).ToArray());
            base.VisitTraitsUse(x);
            _serializer.EndSerialize();
        }

        override public void VisitTraitAdaptationPrecedence(TraitsUse.TraitAdaptationPrecedence x)
        {
            _serializer.StartSerialize(typeof(TraitsUse.TraitAdaptationPrecedence).Name, SerializeSpan(x.Span), 
                new NodeObj("TraitMemberName", (x.TraitMemberName.Item1.HasValue? x.TraitMemberName.Item1.Value.ToString() + "::": string.Empty) + x.TraitMemberName.Item2.Value));
            _serializer.Serialize("IgnoredTypes", x.IgnoredTypes.Select(t => new NodeObj("IgnoredType", t.ToString())).ToArray());
            _serializer.EndSerialize();
        }

        override public void VisitTraitAdaptationAlias(TraitsUse.TraitAdaptationAlias x)
        {
            _serializer.StartSerialize(typeof(TraitsUse.TraitAdaptationAlias).Name, SerializeSpan(x.Span),
                new NodeObj("TraitMemberName", (x.TraitMemberName.Item1.HasValue ? x.TraitMemberName.Item1.Value.ToString() + "::" : string.Empty) + x.TraitMemberName.Item2.Value),
                new NodeObj("NewName", x.NewName ?? string.Empty), new NodeObj("NewModifier", MemberAttributesToString(x.NewModifier ?? PhpMemberAttributes.None)));
            _serializer.EndSerialize();
        }
        override public void VisitGlobalStmt(GlobalStmt x)
        {
            _serializer.StartSerialize(typeof(GlobalStmt).Name, SerializeSpan(x.Span));
            base.VisitGlobalStmt(x);
            _serializer.EndSerialize();
        }

        override public void VisitStaticStmt(StaticStmt x)
        {
            _serializer.StartSerialize(typeof(StaticStmt).Name, SerializeSpan(x.Span));
            foreach (StaticVarDecl v in x.StVarList)
            {
                _serializer.StartSerialize(typeof(StaticVarDecl).Name, SerializeSpan(x.Span), new NodeObj("Name", v.Variable.VarName.Value));
                VisitElement(v.Initializer);
                _serializer.EndSerialize();
            }
            _serializer.EndSerialize();
        }
    }
}
