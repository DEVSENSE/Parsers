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
        /// Updates each <see cref="LangElement.ContainingElement"/> with it's real parent.
        /// </summary>
        public static void UpdateParents(GlobalCode ast)
        {
            if (ast == null)
            {
                return;
            }

            var childvisitor = SyntaxHelpers.GetChildrenProvider();
            var stack = StackObjectPool<ILangElement>.Allocate();

            stack.Push(ast);

            try
            {

                while (stack.TryPop(out var x))
                {
                    var children = childvisitor.GetChildren(x);

                    foreach (var child in children)
                    {
                        Connect(child, x);
                        //Connect(child.Properties.GetPHPDoc(), child); // assigned in SetPHPDoc() already
                        //Debug.Assert(child.Properties.GetPHPDoc()?.ContainingElement == child);

                        stack.Push(child);
                    }
                }

            }
            finally
            {
                StackObjectPool<ILangElement>.Free(stack);
                SyntaxHelpers.ReturnChildrenProvider(childvisitor);
            }
        }

        static void Connect(ILangElement element, ILangElement parent)
        {
            if (element != null)
            {
                element.ContainingElement = parent;
            }
        }
    }
}
