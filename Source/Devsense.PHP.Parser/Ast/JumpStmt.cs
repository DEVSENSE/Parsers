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
	#region JumpStmt

	/// <summary>
	/// Represents a branching (jump) statement (return, continue, break). 
	/// </summary>
    public sealed class JumpStmt : Statement
	{
		/// <summary>
		/// Type of the statement.
		/// </summary>
		public enum Types { Return, Continue, Break };

		private Types type;
        /// <summary>Type of current statement</summary>
        public Types Type { get { return type; } }

		/// <summary>
        /// In case of continue and break, it is number of loop statements to skip. Note that switch is considered to be a loop for this case
        /// In case of return, it represents the returned expression.
        /// Can be null.
		/// </summary>
        public Expression Expression { get { return expr; } internal set { expr = value; } }
		private Expression expr; // can be null

		public JumpStmt(Text.Span span, Types type, Expression expr)
			: base(span)
		{
			this.type = type;
			this.expr = expr;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitJumpStmt(this);
        }
	}

	#endregion

	#region GotoStmt

    public sealed class GotoStmt : Statement
	{
		/// <summary>Label that is target of goto statement</summary>
        public VariableNameRef LabelName { get { return _labelName; } }
        private VariableNameRef _labelName;
        
		public GotoStmt(Text.Span span, VariableNameRef/*!*/labelName)
			: base(span)
		{
            Debug.Assert(!string.IsNullOrEmpty(labelName.Name.Value));
            _labelName = labelName;

        }

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitGotoStmt(this);
        }
	}

	#endregion

	#region LabelStmt

    public sealed class LabelStmt : Statement
	{
        public VariableNameRef Name { get { return _name; } }
		private VariableNameRef _name;

		internal bool IsReferred { get { return isReferred; } set { isReferred = value; } }
		private bool isReferred;
        

		public LabelStmt(Text.Span span, VariableNameRef/*!*/name)
			: base(span)
		{
            Debug.Assert(!string.IsNullOrEmpty(name.Name.Value));

			_name = name;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitLabelStmt(this);
        }
	}

	#endregion
}
