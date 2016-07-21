using PHP.Core.AST;
using PHP.Core.Text;
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

        /// <summary>
        /// Serialize string literal
        /// </summary>
        /// <param name="x"></param>
        override public void VisitStringLiteral(StringLiteral x)
        {
            _serializer.Serialize(typeof(StringLiteral).Name, SerializeSpan(x.Span), new NodeObj("Value", x.Value));
        }

        /// <summary>
        /// Visit expressions in echo statement.
        /// </summary>
        /// <param name="x"></param>
        override public void VisitEchoStmt(EchoStmt x)
        {
            _serializer.StartSerialize(typeof(EchoStmt).Name, SerializeSpan(x.Span));
            VisitExpressions(x.Parameters);
            _serializer.EndSerialize();
        }

        /// <summary>
        /// Visit global scope element and all children.
        /// </summary>
        /// <param name="x">GlobalCode.</param>
        override public void VisitGlobalCode(GlobalCode x)
        {
            _serializer.StartSerialize(typeof(GlobalCode).Name, SerializeSpan(x.Span));
            VisitStatements(x.Statements);
            _serializer.EndSerialize();
        }
    }
}
