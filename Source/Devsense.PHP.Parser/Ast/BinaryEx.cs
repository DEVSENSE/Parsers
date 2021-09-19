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
using System.Diagnostics;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// A binary expression.
    /// </summary>
    public interface IBinaryExpression : IExpression
    {
        /// <summary>
        /// Left operand.
        /// </summary>
        IExpression Left { get; set; }

        /// <summary>
        /// Right operand.
        /// </summary>
        IExpression Right { get; set; }

        /// <summary>
        /// The operator span.
        /// </summary>
        Span OperatorSpan { get; }
    }

    /// <summary>
    /// Binary expression.
    /// </summary>
    public sealed class BinaryEx : Expression, IBinaryExpression
    {
        #region IBinaryExpression

        IExpression IBinaryExpression.Left { get => LeftExpr; set => LeftExpr = (Expression)value; }

        IExpression IBinaryExpression.Right { get => RightExpr; set => RightExpr = (Expression)value; }

        public Span OperatorSpan => SpanUtils.SpanIntermission(leftExpr.Span, rightExpr.Span);

        #endregion

        #region Fields & Properties

        public Expression/*!*/ LeftExpr { get { return leftExpr; } internal set { Debug.Assert(value != null); leftExpr = value; } }
        private Expression/*!*/ leftExpr;

        public Expression/*!*/ RightExpr { get { return rightExpr; } internal set { Debug.Assert(value != null); rightExpr = value; } }
        private Expression/*!*/ rightExpr;

        public override Operations Operation { get; }

        #endregion

        #region Construction

        public BinaryEx(Span span, Operations operation, Expression/*!*/ leftExpr, Expression/*!*/ rightExpr)
            : base(span)
        {
            Debug.Assert(leftExpr != rightExpr, "LeftExpr == RightExpr");

            this.Operation = operation;
            this.LeftExpr = leftExpr;
            this.RightExpr = rightExpr;
        }

        #endregion

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitBinaryEx(this);
        }
    }
}