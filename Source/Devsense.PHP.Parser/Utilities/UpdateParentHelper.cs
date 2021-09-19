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
using Devsense.PHP.Syntax.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devsense.PHP.Utilities
{
    /// <summary>
    /// Helper updating reference to parent nodes of all elements (ensures correctly doubly-linked tree).
    /// </summary>
    static class UpdateParentHelper
    {
        /// <summary>
        /// Collects element child elements.
        /// </summary>
        class GetChildrenVisitor : TreeVisitor
        {
            /// <summary>
            /// Buffer.
            /// </summary>
            LangElement[] _children = EmptyArray<LangElement>.Instance;

            int _childrenCount = 0;

            void AddChild(LangElement element)
            {
                if (element != null)
                {
                    if (_childrenCount == _children.Length)
                    {
                        Array.Resize(ref _children, (_children.Length + 2) * 2);
                    }

                    _children[_childrenCount++] = element;
                }
            }

            /// <summary>
            /// Gets list of direct child nodes.
            /// Cannot be <c>null</c>.
            /// </summary>
            public ReadOnlySpan<LangElement> GetChildren(LangElement element)
            {
                if (element != null)
                {
                    _childrenCount = 0;
                    element.VisitMe(this);
                    return _children.AsSpan(0, _childrenCount);
                }

                //
                return ReadOnlySpan<LangElement>.Empty;
            }

            public override void VisitElement(LangElement element)
            {
                AddChild(element);

                // do not visit the child
            }
        }

        /// <summary>
        /// Updates each <see cref="LangElement.ContainingElement"/> with it's real parent.
        /// </summary>
        public static void UpdateParents(GlobalCode ast)
        {
            if (ast == null)
            {
                return;
            }

            var queue = new Queue<LangElement>(32);
            var childvisitor = new GetChildrenVisitor();

            queue.Enqueue(ast);

            while (queue.TryDequeue(out var x))
            {
                var children = childvisitor.GetChildren(x);

                foreach (var child in children)
                {
                    Connect(child, x);
                    Connect(child.GetPHPDoc(), child);

                    queue.Enqueue(child);
                }
            }
        }

        static void Connect(LangElement element, LangElement parent)
        {
            if (element != null)
            {
                element.ContainingElement = parent;
            }
        }
    }
}
