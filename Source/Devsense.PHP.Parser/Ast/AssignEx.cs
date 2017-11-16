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
    /// Base class for assignment expressions (by-value and by-ref).
    /// </summary>
    public abstract class AssignEx : Expression
    {
        internal override bool AllowsPassByReference { get { return true; } }

        /// <summary>Target of assignment</summary>
        public VarLikeConstructUse LValue { get { return lvalue; } }
        protected VarLikeConstructUse lvalue;

        protected AssignEx(Text.Span p) : base(p) { }
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
        public override Operations Operation { get { return operation; } }
        internal Operations operation;

        /// <summary>Expression being assigned</summary>
        public Expression/*!*/RValue { get { return rvalue; } }
        Expression/*!*/ rvalue;

        public ValueAssignEx(Text.Span span, Operations operation, VarLikeConstructUse/*!*/ lvalue, Expression/*!*/ rvalue)
            : base(span)
        {
            this.lvalue = lvalue;
            this.rvalue = rvalue;
            this.operation = operation;
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

        /// <summary>Expression being assigned</summary>
        public Expression/*!*/RValue { get { return rvalue; } }
        Expression/*!*/ rvalue;

        /// <summary>
        /// Create new assignment.
        /// </summary>
        /// <param name="span">Entire span.</param>
        /// <param name="lvalue">Assigned variable.</param>
        /// <param name="rvalue">Assigned value.</param>
        public RefAssignEx(Text.Span span, VarLikeConstructUse/*!*/ lvalue, Expression/*!*/ rvalue)
            : base(span)
        {
            Debug.Assert(rvalue != null);
            Debug.Assert(rvalue.AllowsPassByReference);

            this.lvalue = lvalue;
            this.rvalue = rvalue;
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
