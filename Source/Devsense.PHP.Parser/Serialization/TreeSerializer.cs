using System;
using System.Collections.Generic;
using System.Linq;

using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast.Serialization
{
    public class TreeSerializer : TreeVisitor
    {
        INodeWriter _serializer;

        public TreeSerializer(INodeWriter serializer) : base()
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

        void SerializeOptionalProperty(string name, LangElement x)
        {
            if (x != null)
            {
                _serializer.StartSerialize(name);
                VisitElement(x);
                _serializer.EndSerialize();
            }
        }

        void SerializeItem(ValueItem item)
        {
            _serializer.StartSerialize("Item");
            SerializeOptionalProperty("Index", item.Index);
            SerializeOptionalProperty("ValueExpr", item.ValueExpr);
            _serializer.EndSerialize();
        }

        void SerializeItem(RefItem item)
        {
            _serializer.StartSerialize("Item");
            SerializeOptionalProperty("Index", item.Index);
            SerializeOptionalProperty("RefToGet", item.RefToGet);
            _serializer.EndSerialize();
        }

        void SerializePHPDoc(PHPDocBlock doc)
        {
            if (doc != null)
                _serializer.Serialize("PHPDoc", SerializeSpan(doc.Span), new NodeObj("Comment", doc.ToString()));
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
            _serializer.Serialize(typeof(DoubleLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", x.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)));
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
        override public void VisitHaltCompiler(HaltCompiler x)
        {
            _serializer.Serialize(typeof(HaltCompiler).Name, SerializeSpan(x.Span));
        }

        /// <summary>
        /// Visit global scope element and all children.
        /// </summary>
        /// <param name="x">GlobalCode.</param>
        override public void VisitGlobalCode(GlobalCode x)
        {
            _serializer.StartSerialize(typeof(GlobalCode).Name, SerializeSpan(x.Span), SerializeNamingContext(x.ContainingSourceUnit.Naming));
            base.VisitGlobalCode(x);
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit namespace statements.
        /// </summary>
        /// <param name="x"></param>
        override public void VisitNamespaceDecl(NamespaceDecl x)
        {
            _serializer.StartSerialize(typeof(NamespaceDecl).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.QualifiedName.QualifiedName.NamespacePhpName),
                new NodeObj("SimpleSyntax", x.IsSimpleSyntax.ToString()),
                SerializeNamingContext(x.Naming));
            SerializePHPDoc(x.PHPDoc);
            SerializeOptionalProperty("Body", x.Body);
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit constant declarations.
        /// </summary>
        /// <param name="x"></param>
        override public void VisitGlobalConstDeclList(GlobalConstDeclList x)
        {
            _serializer.StartSerialize(typeof(GlobalConstDeclList).Name, SerializeSpan(x.Span));
            SerializePHPDoc(x.PHPDoc);
            base.VisitGlobalConstDeclList(x);
            _serializer.EndSerialize();
        }

        override public void VisitGlobalConstantDecl(GlobalConstantDecl x)
        {
            _serializer.StartSerialize(typeof(GlobalConstantDecl).Name, SerializeSpan(x.Span),
                new NodeObj("NameIsConditional", x.IsConditional.ToString()), new NodeObj("Name", x.Name.Name.Value));
            SerializePHPDoc(x.PHPDoc);
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
                new NodeObj("Name", x.QualifiedName.ToString()), new NodeObj("FallbackQualifiedName", x.FallbackQualifiedName.ToString()));
            base.VisitDirectFcnCall(x);
            _serializer.EndSerialize();
        }
        override public void VisitIndirectFcnCall(IndirectFcnCall x)
        {
            _serializer.StartSerialize(typeof(IndirectFcnCall).Name, SerializeSpan(x.Span));
            base.VisitIndirectFcnCall(x);
            _serializer.EndSerialize();
        }
        override public void VisitDirectStMtdCall(DirectStMtdCall x)
        {
            _serializer.StartSerialize(typeof(DirectStMtdCall).Name, SerializeSpan(x.Span),
                new NodeObj("ClassName", x.TargetType.QualifiedName.ToString()),
                new NodeObj("MethodName", x.MethodName.Name.Value));
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
            SerializeOptionalProperty("Body", x.Body);
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
        override public void VisitDirectStFldUse(DirectStFldUse x)
        {
            _serializer.StartSerialize(typeof(DirectStFldUse).Name, SerializeSpan(x.Span),
                new NodeObj("PropertyName", x.PropertyName.Value));
            base.VisitDirectStFldUse(x);
            _serializer.EndSerialize();
        }
        override public void VisitIndirectStFldUse(IndirectStFldUse x)
        {
            _serializer.StartSerialize(typeof(IndirectStFldUse).Name, SerializeSpan(x.Span));
            base.VisitIndirectStFldUse(x);
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
            VisitList(x.InitExList);
            _serializer.EndSerialize();
            _serializer.StartSerialize("CondExList");
            VisitList(x.CondExList);
            _serializer.EndSerialize();
            _serializer.StartSerialize("ActionExList");
            VisitList(x.ActionExList);
            _serializer.EndSerialize();
            SerializeOptionalProperty("Body", x.Body);
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
            SerializeOptionalProperty("Enumeree", x.Enumeree);
            if(x.KeyVariable != null) SerializeOptionalProperty("KeyVariable", x.KeyVariable.Target);
            SerializeOptionalProperty("ValueVariable", x.ValueVariable.Target);
            SerializeOptionalProperty("Body", x.Body);
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
            SerializeOptionalProperty("Body", x.Body);

            _serializer.StartSerialize("Catches");
            if (x.Catches != null)
                foreach (CatchItem c in x.Catches)
                    VisitElement(c);
            _serializer.EndSerialize();
            VisitElement(x.FinallyItem);
            _serializer.EndSerialize();
        }
        override public void VisitCatchItem(CatchItem x)
        {
            _serializer.StartSerialize(typeof(CatchItem).Name, SerializeSpan(x.Span));
            SerializeOptionalProperty("TypeRef", x.TypeRef);
            SerializeOptionalProperty("Variable", x.Variable);
            SerializeOptionalProperty("Body", x.Body);
            _serializer.EndSerialize();
        }
        override public void VisitFinallyItem(FinallyItem x)
        {
            _serializer.StartSerialize(typeof(FinallyItem).Name, SerializeSpan(x.Span));
            SerializeOptionalProperty("Body", x.Body);
            _serializer.EndSerialize();
        }
        override public void VisitDirectTypeRef(DirectTypeRef x)
        {
            _serializer.StartSerialize(typeof(DirectTypeRef).Name, SerializeSpan(x.Span), 
                new NodeObj("ClassName", x.QualifiedName.ToString()));
            _serializer.EndSerialize();
        }
        override public void VisitIndirectTypeRef(IndirectTypeRef x)
        {
            _serializer.StartSerialize(typeof(IndirectTypeRef).Name, SerializeSpan(x.Span));
            base.VisitIndirectTypeRef(x);
            _serializer.EndSerialize();
        }

        override public void VisitPrimitiveTypeRef(PrimitiveTypeRef x)
        {
            _serializer.Serialize(typeof(PrimitiveTypeRef).Name, SerializeSpan(x.Span),
                new NodeObj("QualifiedName", x.QualifiedName.ToString()));
        }

        override public void VisitNullableTypeRef(NullableTypeRef x)
        {
            _serializer.StartSerialize(typeof(NullableTypeRef).Name, SerializeSpan(x.Span));
            base.VisitNullableTypeRef(x);
            _serializer.EndSerialize();
        }
        override public void VisitMultipleTypeRef(MultipleTypeRef x)
        {
            _serializer.StartSerialize(typeof(MultipleTypeRef).Name, SerializeSpan(x.Span));
            base.VisitMultipleTypeRef(x);
            _serializer.EndSerialize();
        }
        override public void VisitGenericTypeRef(GenericTypeRef x)
        {
            _serializer.StartSerialize(typeof(GenericTypeRef).Name, SerializeSpan(x.Span));
            SerializeOptionalProperty("TargetType", x.TargetType);
            if (x.GenericParams != null && x.GenericParams.Count > 0)
            {
                _serializer.StartSerialize("GenericParams");
                foreach (var param in x.GenericParams)
                    VisitElement(param);
                _serializer.EndSerialize();
            }
            _serializer.EndSerialize();
        }
        override public void VisitGotoStmt(GotoStmt x)
        {
            _serializer.Serialize(typeof(GotoStmt).Name, SerializeSpan(x.Span), new NodeObj("LabelName", x.LabelName.Name.Value));
        }
        override public void VisitLabelStmt(LabelStmt x)
        {
            _serializer.Serialize(typeof(LabelStmt).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Name.Value));
        }
        override public void VisitNewEx(NewEx x)
        {
            _serializer.StartSerialize(typeof(NewEx).Name, SerializeSpan(x.Span));
            base.VisitNewEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitFunctionDecl(FunctionDecl x)
        {
            _serializer.StartSerialize(typeof(FunctionDecl).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Name.Value), new NodeObj("IsConditional", x.IsConditional.ToString()));
            SerializePHPDoc(x.PHPDoc);
            _serializer.StartSerialize("FormalParams");
            // function parameters
            foreach (FormalParam p in x.Signature.FormalParams)
                VisitElement(p);
            _serializer.EndSerialize();
            
            SerializeOptionalProperty("Body", x.Body);

            SerializeOptionalProperty("ReturnType", x.ReturnType);
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit <see cref="LambdaFunctionExpr"/> expression.
        /// </summary>
        override public void VisitLambdaFunctionExpr(LambdaFunctionExpr x)
        {
            _serializer.StartSerialize(typeof(LambdaFunctionExpr).Name, SerializeSpan(x.Span));
            SerializePHPDoc(x.PHPDoc);
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
            VisitElement(x.Body);
            _serializer.EndSerialize();

            if (x.ReturnType != null)
            {
                _serializer.StartSerialize("ReturnType");
                VisitElement(x.ReturnType);
                _serializer.EndSerialize();
            }
            _serializer.EndSerialize();
        }
        override public void VisitFormalParam(FormalParam x)
        {
            _serializer.StartSerialize(typeof(FormalParam).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Name.Value), 
                new NodeObj("PassedByRef", x.PassedByRef.ToString()), new NodeObj("IsVariadic", x.IsVariadic.ToString()));
            SerializeOptionalProperty("TypeHint", x.TypeHint);
            SerializeOptionalProperty("InitValue", x.InitValue);
            _serializer.EndSerialize();
        }
        override public void VisitYieldEx(YieldEx x)
        {
            _serializer.StartSerialize(typeof(YieldEx).Name, SerializeSpan(x.Span));
            base.VisitYieldEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitYieldFromEx(YieldFromEx x)
        {
            _serializer.StartSerialize(typeof(YieldFromEx).Name, SerializeSpan(x.Span));
            base.VisitYieldFromEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitDeclareStmt(DeclareStmt x)
        {
            _serializer.StartSerialize(typeof(DeclareStmt).Name, SerializeSpan(x.Span));
            base.VisitDeclareStmt(x);
            _serializer.EndSerialize();
        }
        override public void VisitNamedTypeDecl(NamedTypeDecl x)
        {
            _serializer.StartSerialize(typeof(NamedTypeDecl).Name, SerializeSpan(x.Span), 
                new NodeObj("Name", x.Name.Name.Value), new NodeObj("MemberAttributes", MemberAttributesToString(x.MemberAttributes)),
                new NodeObj("IsConditional", x.IsConditional.ToString()));
            if (x.BaseClass.HasValue)
                _serializer.Serialize("BaseClassName", new NodeObj("Name", x.BaseClass.QualifiedName.ToString()));
            if (x.ImplementsList != null && x.ImplementsList.Length > 0)
                _serializer.Serialize("ImplementsList", x.ImplementsList.Select(n => new NodeObj("Name", n.QualifiedName.ToString())).ToArray());
            SerializePHPDoc(x.PHPDoc);
            base.VisitNamedTypeDecl(x);
            _serializer.EndSerialize();
        }
        override public void VisitAnonymousTypeDecl(AnonymousTypeDecl x)
        {
            _serializer.StartSerialize(typeof(AnonymousTypeDecl).Name, SerializeSpan(x.Span),
                new NodeObj("MemberAttributes", MemberAttributesToString(x.MemberAttributes)),
                new NodeObj("IsConditional", x.IsConditional.ToString()));
            if (x.BaseClass.HasValue)
                _serializer.Serialize("BaseClassName", new NodeObj("Name", x.BaseClass.QualifiedName.ToString()));
            if (x.ImplementsList != null && x.ImplementsList.Length > 0)
                _serializer.Serialize("ImplementsList", x.ImplementsList.Select(n => new NodeObj("Name", n.QualifiedName.ToString())).ToArray());
            SerializePHPDoc(x.PHPDoc);
            base.VisitAnonymousTypeDecl(x);
            _serializer.EndSerialize();
        }
        override public void VisitFieldDeclList(FieldDeclList x)
        {
            _serializer.StartSerialize(typeof(FieldDeclList).Name, SerializeSpan(x.Span));
            SerializePHPDoc(x.PHPDoc);
            foreach (FieldDecl f in x.Fields)
                VisitFieldDecl(f, x.Modifiers);
            _serializer.EndSerialize();
        }
        public void VisitFieldDecl(FieldDecl x, PhpMemberAttributes attributes)
        {
            _serializer.StartSerialize(typeof(FieldDecl).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.Name.Value), new NodeObj("MemberAttributes", MemberAttributesToString(attributes)));
            SerializePHPDoc(x.PHPDoc);
            VisitElement(x.Initializer);
            _serializer.EndSerialize();
        }
        override public void VisitConstDeclList(ConstDeclList x)
        {
            _serializer.StartSerialize(typeof(ConstDeclList).Name, SerializeSpan(x.Span));
            SerializePHPDoc(x.PHPDoc);
            foreach (ClassConstantDecl c in x.Constants)
                VisitClassConstantDecl(c, x.Modifiers);
            _serializer.EndSerialize();
        }
        public void VisitClassConstantDecl(ClassConstantDecl x, PhpMemberAttributes attributes)
        {
            _serializer.StartSerialize(typeof(ClassConstantDecl).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.Name.Name.Value), new NodeObj("MemberAttributes", MemberAttributesToString(attributes)));
            SerializePHPDoc(x.PHPDoc);
            VisitElement(x.Initializer);
            _serializer.EndSerialize();
        }
        override public void VisitMethodDecl(MethodDecl x)
        {
            _serializer.StartSerialize(typeof(MethodDecl).Name, SerializeSpan(x.Span), new NodeObj("Name", x.Name.Name.Value), 
                new NodeObj("Modifiers", MemberAttributesToString(x.Modifiers)));
            SerializePHPDoc(x.PHPDoc);
            _serializer.StartSerialize("FormalParams");
            // method parameters
            foreach (FormalParam p in x.Signature.FormalParams)
                VisitElement(p);
            _serializer.EndSerialize();
            
            SerializeOptionalProperty("Body", x.Body);

            SerializeOptionalProperty("ReturnType", x.ReturnType);
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

        override public void VisitVarLikeConstructUse(VarLikeConstructUse x)
        {
            SerializeOptionalProperty("IsMemberOf", x.IsMemberOf);
        }
        override public void VisitGlobalConstUse(GlobalConstUse x)
        {
            _serializer.StartSerialize(typeof(GlobalConstUse).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.Name.ToString()), new NodeObj("FallbackName", x.FallbackName.ToString()));
            base.VisitGlobalConstUse(x);
            _serializer.EndSerialize();
        }
        override public void VisitClassConstUse(ClassConstUse x)
        {
            _serializer.StartSerialize(typeof(ClassConstUse).Name, SerializeSpan(x.Span),
                new NodeObj("Name", x.Name.ToString()));
            base.VisitClassConstUse(x);
            _serializer.EndSerialize();
        }
        override public void VisitItemUse(ItemUse x)
        {
            _serializer.StartSerialize(typeof(ItemUse).Name, SerializeSpan(x.Span));
            SerializeOptionalProperty("Array", x.Array);
            SerializeOptionalProperty("Index", x.Index);
            VisitVarLikeConstructUse(x);
            _serializer.EndSerialize();
        }
        override public void VisitListEx(ListEx x)
        {
            _serializer.StartSerialize(typeof(ListEx).Name, SerializeSpan(x.Span));
            foreach (var item in x.Items)
                if (item is ValueItem)
                    SerializeItem((ValueItem)item);
                else
                    SerializeItem((RefItem)item);
            _serializer.EndSerialize();
        }

        override public void VisitArrayEx(ArrayEx x)
        {
            _serializer.StartSerialize(typeof(ArrayEx).Name, SerializeSpan(x.Span));
            foreach (var item in x.Items)
                if (item is ValueItem)
                    SerializeItem((ValueItem)item);
                else SerializeItem((RefItem)item);
            _serializer.EndSerialize();
        }

        override public void VisitEmptyEx(EmptyEx x)
        {
            _serializer.StartSerialize(typeof(EmptyEx).Name, SerializeSpan(x.Span));
            base.VisitEmptyEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitExitEx(ExitEx x)
        {
            _serializer.StartSerialize(typeof(ExitEx).Name, SerializeSpan(x.Span));
            base.VisitExitEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitEvalEx(EvalEx x)
        {
            _serializer.StartSerialize(typeof(EvalEx).Name, SerializeSpan(x.Span));
            base.VisitEvalEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitIssetEx(IssetEx x)
        {
            _serializer.StartSerialize(typeof(IssetEx).Name, SerializeSpan(x.Span));
            base.VisitIssetEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitIncludingEx(IncludingEx x)
        {
            _serializer.StartSerialize(typeof(IncludingEx).Name, SerializeSpan(x.Span), 
                new NodeObj("InclusionType", x.InclusionType.ToString()), 
                new NodeObj("IsConditional", x.IsConditional.ToString()));
            base.VisitIncludingEx(x);
            _serializer.EndSerialize();
        }

        override public void VisitAssertEx(AssertEx x)
        {
            _serializer.StartSerialize(typeof(AssertEx).Name, SerializeSpan(x.Span));
            base.VisitAssertEx(x);
            _serializer.EndSerialize();
        }
        override public void VisitInstanceOfEx(InstanceOfEx x)
        {
            _serializer.StartSerialize(typeof(InstanceOfEx).Name, SerializeSpan(x.Span));
            base.VisitInstanceOfEx(x);
            _serializer.EndSerialize();
        }
    }
}
