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
            base.VisitDirectFcnCall(x);
            _serializer.EndSerialize();
        }
        override public void VisitActualParam(ActualParam x)
        {
            _serializer.StartSerialize(typeof(ActualParam).Name, SerializeSpan(x.Span));
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
    }
}
