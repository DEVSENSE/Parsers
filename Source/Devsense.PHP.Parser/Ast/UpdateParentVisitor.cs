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

                base.VisitElement(element.GetPHPDoc());
                base.VisitElement(element);

                _parents.RemoveFirst();
            }
        }
    }
}
