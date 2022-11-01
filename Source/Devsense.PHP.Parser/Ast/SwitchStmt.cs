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
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
	#region SwitchStmt

	/// <summary>
	/// Switch statement.
	/// </summary>
	public sealed class SwitchStmt : Statement
	{
		/// <summary>Value to switch by</summary>
        public Expression/*!*/ SwitchValue { get; internal set; }

        /// <summary>
        /// Position of the header parentheses.
        /// </summary>
        public Text.Span ValueSpan { get; }

        /// <summary>Body of switch statement</summary>
        public SwitchItem[]/*!*/ SwitchItems { get; }

        /// <summary>
        /// A token that closes the block.
        /// Can be <c>}</c>, <c>endswitch</c>.
        /// </summary>
        public Tokens ClosingToken { get; }

		/// <summary>
		/// <see cref="ClosingToken"/> position.
		/// </summary>
        public Text.Span ClosingTokenSpan { get; }

        public SwitchStmt(Text.Span span,
			Expression/*!*/ switchValue,
            Text.Span valueSpan,
            IList<SwitchItem>/*!*/ switchItems,
			Tokens closingToken, 
			Text.Span closingTokenSpan)
			: base(span)
		{
			Debug.Assert(switchValue != null && switchItems != null);

			this.SwitchValue = switchValue;
			this.ValueSpan = valueSpan;
			this.SwitchItems = switchItems.AsArray();
			this.ClosingToken = closingToken;
			this.ClosingTokenSpan = closingTokenSpan;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitSwitchStmt(this);
        }
	}

	#endregion

	#region SwitchItem

	/// <summary>
	/// Base class for switch case/default items.
	/// </summary>
    public abstract class SwitchItem : LangElement
	{
        protected readonly Statement[]/*!*/ statements;
        /// <summary>Statements in this part of switch</summary>
        public Statement[]/*!*/ Statements { get { return statements; } }

		protected SwitchItem(Text.Span span, IList<Statement>/*!*/ statements)
			: base(span)
		{
			Debug.Assert(statements != null);
			this.statements = statements.AsArray();
		}
	}

	/// <summary>
	/// Switch <c>case</c> item.
	/// </summary>
    public sealed class CaseItem : SwitchItem
	{
        /// <summary>Value to compare with swich expression</summary>
        public Expression CaseVal { get { return caseVal; } internal set { caseVal = value; } }
        private Expression caseVal;

		public CaseItem(Text.Span span, Expression/*!*/ caseVal, IList<Statement>/*!*/ statements)
			: base(span, statements)
		{
			Debug.Assert(caseVal != null);
			this.caseVal = caseVal;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitCaseItem(this);
        }
	}

	/// <summary>
	/// Switch <c>default</c> item.
	/// </summary>
    public sealed class DefaultItem : SwitchItem
	{
		public DefaultItem(Text.Span span, IList<Statement>/*!*/ statements)
			: base(span, statements)
		{
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitDefaultItem(this);
        }
    }

	#endregion
}
