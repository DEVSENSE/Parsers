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

using Devsense.PHP.Text;
using System;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Conditional expression.
    /// </summary>
    public sealed class ConditionalEx : Expression
    {
        public override Span Span
        {
            get => Span.FromBounds(CondExpr.Span.Start, FalseExpr.Span.End);
            protected set { }
        }

        public override Operations Operation { get { return Operations.Conditional; } }

        /// <summary>Condition</summary>
        public Expression/*!*/ CondExpr { get; }

        /// <summary>Expression evaluated when <see cref="CondExpr"/> is true. Can be <c>null</c> in case of ternary shortcut (?:).</summary>
        public Expression TrueExpr { get; }

        /// <summary><summary>Expression evaluated when <see cref="CondExpr"/> is false</summary></summary>
        public Expression/*!*/ FalseExpr { get; }

        public ConditionalEx(Text.Span span, Expression/*!*/ condExpr, Expression trueExpr, Expression/*!*/ falseExpr)
            : base(span)
        {
            Debug.Assert(condExpr != null);
            // Debug.Assert(trueExpr != null); // allowed to enable ternary shortcut
            Debug.Assert(falseExpr != null);

            this.CondExpr = condExpr;
            this.TrueExpr = trueExpr;
            this.FalseExpr = falseExpr;
        }

        public ConditionalEx(Expression/*!*/ condExpr, Expression/*!*/ trueExpr, Expression/*!*/ falseExpr)
            : this(Text.Span.Invalid, condExpr, trueExpr, falseExpr)
        {
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitConditionalEx(this);
        }
    }
}

