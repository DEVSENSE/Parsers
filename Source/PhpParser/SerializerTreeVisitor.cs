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
    }
}
