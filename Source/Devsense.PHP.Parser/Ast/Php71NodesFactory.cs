using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax.Ast
{
    class Php71NodesFactory : BasicNodesFactory
    {
        public Php71NodesFactory(SourceUnit sourceUnit) : base(sourceUnit)
        {
        }

        public override LangElement TypeReference(Span span, QualifiedName className)
        {
            return className.IsSimpleName && (Equals(QualifiedName.Void) || Equals(QualifiedName.Iterable))
                ? (TypeRef)new PrimitiveTypeRef(span, new PrimitiveTypeName(className))
                : base.TypeReference(span, className);
        }
    }
}
