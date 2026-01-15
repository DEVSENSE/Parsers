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
    /// Unary expression.
    /// </summary>
    public sealed class UnaryEx : Expression
    {
        #region Fields & Properties

        public override Span Span
        {
            get => _span_start < 0 ? Span.Invalid : Span.FromBounds(_span_start, this.Expr.Span.End);
            protected set => _span_start = value.IsValid ? value.Start : -1;
        }
        int _span_start;

        public override Operations Operation { get; }

        /// <summary>Expression the operator is applied on</summary>
        public Expression /*!*/ Expr { get; }

        #endregion

        #region Construction

        public UnaryEx(Text.Span span, Operations operation, Expression/*!*/ expr)
            : base(span)
        {
            this.Operation = operation;
            this.Expr = expr ?? throw new ArgumentNullException(nameof(expr));
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitUnaryEx(this);
        }
    }
}