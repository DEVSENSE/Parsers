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
	/// Binary expression.
	/// </summary>
	public sealed class BinaryEx : Expression
    {
        #region Fields & Properties

        public Expression/*!*/ LeftExpr { get { return leftExpr; } internal set { leftExpr = value; } }
		private Expression/*!*/ leftExpr;

        public Expression/*!*/ RightExpr { get { return rightExpr; } internal set { rightExpr = value; } }
		private Expression/*!*/ rightExpr;

        public override Operations Operation { get { return operation; } }
		private Operations operation;

        #endregion

        #region Construction

        public BinaryEx(Text.Span span, Operations operation, Expression/*!*/ leftExpr, Expression/*!*/ rightExpr)
			: base(span)
		{
			Debug.Assert(leftExpr != null && rightExpr != null);
			this.operation = operation;
			this.leftExpr = leftExpr;
			this.rightExpr = rightExpr;
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