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
	/// Unary expression.
	/// </summary>
    public sealed class UnaryEx : Expression
    {
        #region Fields & Properties

        public override Operations Operation { get { return operation; } }
		private Operations operation;

		/// <summary>Expression the operator is applied on</summary>
        public Expression /*!*/ Expr { get { return expr; } internal set { expr = value; } }
        private Expression/*!*/ expr;

        #endregion

        #region Construction

        public UnaryEx(Text.Span span, Operations operation, Expression/*!*/ expr)
			: base(span)
		{
			Debug.Assert(expr != null);
			this.operation = operation;
			this.expr = expr;
		}

		public UnaryEx(Operations operation, Expression/*!*/ expr)
			: this(Text.Span.Invalid, operation, expr)
		{
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