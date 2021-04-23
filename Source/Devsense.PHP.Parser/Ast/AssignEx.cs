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

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// An assignment expression.
    /// </summary>
    public interface IAssignmentEx : IExpression
    {
        /// <summary>
        /// The left side of the assignment.
        /// </summary>
        IExpression Target { get; }

        /// <summary>
        /// The value of the assignment.
        /// </summary>
        IExpression RValue { get; }
    }

    /// <summary>
    /// Base class for assignment expressions (by-value and by-ref).
    /// </summary>
    public abstract class AssignEx : Expression, IAssignmentEx
    {
        internal override bool AllowsPassByReference => true;

        /// <summary>Target of assignment</summary>
        public VarLikeConstructUse LValue { get; }

        /// <summary>Expression being assigned</summary>
        public Expression/*!*/RValue { get; }

        protected AssignEx(Text.Span p, VarLikeConstructUse target, Expression rValue) : base(p)
        {
            LValue = target ?? throw new ArgumentNullException(nameof(target));
            RValue = rValue ?? throw new ArgumentNullException(nameof(rValue));
        }

        #region IAssignmentEx

        IExpression IAssignmentEx.Target => LValue;

        IExpression IAssignmentEx.RValue => RValue;

        #endregion
    }

    #region ValueAssignEx

    /// <summary>
    /// By-value assignment expression with possibly associated operation.
    /// </summary>
    /// <remarks>
    /// Implements PHP operators: <c>=  +=  -=  *=  /=  %=  .= =.  &amp;=  |=  ^=  &lt;&lt;=  &gt;&gt;=</c>.
    /// </remarks>
    public sealed class ValueAssignEx : AssignEx
    {
        public override Operations Operation { get; }

        public ValueAssignEx(Text.Span span, Operations operation, VarLikeConstructUse/*!*/ lvalue, Expression/*!*/ rvalue)
            : base(span, lvalue, rvalue)
        {
            this.Operation = operation;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitValueAssignEx(this);
        }
    }

    #endregion

    #region RefAssignEx

    /// <summary>
    /// By-reference assignment expression (<c>&amp;=</c> PHP operator).
    /// </summary>
    public sealed class RefAssignEx : AssignEx
    {
        /// <summary>
        /// Assignment operation.
        /// </summary>
        public override Operations Operation => Operations.AssignRef;

        /// <summary>
        /// Create new assignment.
        /// </summary>
        /// <param name="span">Entire span.</param>
        /// <param name="lvalue">Assigned variable.</param>
        /// <param name="rvalue">Assigned value.</param>
        public RefAssignEx(Text.Span span, VarLikeConstructUse/*!*/ lvalue, Expression/*!*/ rvalue)
            : base(span, lvalue, rvalue)
        {
            Debug.Assert(rvalue.AllowsPassByReference);
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitRefAssignEx(this);
        }
    }

    #endregion
}
