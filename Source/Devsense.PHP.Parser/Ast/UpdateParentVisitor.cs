using Devsense.PHP.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Visitor implementation that updates reference to parent nodes of all elements.
    /// </summary>
    internal sealed class UpdateParentVisitor : TreeVisitor
    {
        readonly LinkedList<LangElement> _parents = new LinkedList<LangElement>();

        /// <summary>
        /// Gets the element on top of stack.
        /// </summary>
        LangElement ContainingElement => _parents.First?.Value;

        public static void UpdateParents(GlobalCode x, LangElement root = null)
        {
            var visitor = new UpdateParentVisitor();

            visitor.VisitElement(x);
            x.ContainingElement = root;
        }

        public override void VisitGlobalCode(GlobalCode x)
        {
            _parents.AddFirst(x);

            base.VisitGlobalCode(x);

            _parents.RemoveFirst();
        }

        public override void VisitElement(LangElement element)
        {
            if (element != null)
            {
                element.ContainingElement = this.ContainingElement;

                _parents.AddFirst(element);

                base.VisitElement(element);

                _parents.RemoveFirst();
            }
        }
    }
}
