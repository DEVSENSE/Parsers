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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
	/// <summary>
	/// Represents an if-statement.
	/// </summary>
	public sealed class IfStmt : Statement
	{
		/// <summary>
		/// List of conditions including the if-conditions and the final else.
		/// </summary>
		private List<ConditionalStmt>/*!!*/ conditions;
        public List<ConditionalStmt>/*!!*/ Conditions { get { return conditions; } internal set { conditions = value; } }

		public IfStmt(Text.Span span, List<ConditionalStmt>/*!!*/ conditions)
			: base(span)
		{
			Debug.Assert(conditions != null && conditions.Count > 0);
			Debug.Assert(conditions.All((x) => x != null));
			this.conditions = conditions;
		}

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIfStmt(this);
        }
	}

	public sealed class ConditionalStmt : AstNode
	{
		/// <summary>
		/// Condition or a <B>null</B> reference for the case of "else" branch.
		/// </summary>
		public Expression Condition { get { return condition; } internal set { condition = value; } }
		private Expression condition;

        /// <summary>
        /// Position of the condition including the parentheses.
        /// </summary>
        public Text.Span ConditionPosition { get { return _positionOffset; } }
        private Text.Span _positionOffset;

        public Statement/*!*/ Statement { get { return statement; } internal set { statement = value; } }
		private Statement/*!*/ statement;

        /// <summary>
        /// Beginning of <see cref="ConditionalStmt"/>.
        /// </summary>
        public readonly Text.Span Span;

        public ConditionalStmt(Text.Span span, Expression condition, Text.Span conditionSpan, Statement/*!*/ statement)
		{
            this.Span = span;
			this.condition = condition;
			this.statement = statement;
            this._positionOffset = conditionSpan;

        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        internal void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitConditionalStmt(this);
        }
	}
}
