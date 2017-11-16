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

using System;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// <c>__halt_compiler</c> element.
    /// </summary>
    public sealed class HaltCompiler : Statement
    {
        /// <summary>
        /// Initializes the element.
        /// </summary>
        public HaltCompiler(Text.Span span) : base(span)
        {
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitHaltCompiler(this);
        }
    }
}
