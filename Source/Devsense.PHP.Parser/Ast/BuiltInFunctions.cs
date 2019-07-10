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
using System.Collections.Generic;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    #region IncludingEx

    /// <summary>
	/// Inclusion expression (include, require, synthetic auto-inclusion nodes).
	/// </summary>
	public sealed class IncludingEx : Expression
	{
        public override Operations Operation { get { return Operations.Inclusion; } }

		/// <summary>
		/// An argument of the inclusion.
		/// </summary>
        public Expression/*!*/ Target { get { return fileNameEx; } set { fileNameEx = value; } }
		private Expression/*!*/ fileNameEx;

		/// <summary>
		/// A type of an inclusion (include, include-once, ...).
		/// </summary>
		public InclusionTypes InclusionType { get { return inclusionType; } }
		private InclusionTypes inclusionType;

		/// <summary>
		/// Whether the inclusion is conditional.
		/// </summary>
		public bool IsConditional { get { return isConditional; } internal set { isConditional = value; } }
		private bool isConditional;

		public IncludingEx(bool isConditional, Text.Span span,
			InclusionTypes inclusionType, Expression/*!*/ fileName)
            : base(span)
		{
			Debug.Assert(fileName != null);

			this.inclusionType = inclusionType;
			this.fileNameEx = fileName;
			this.isConditional = isConditional;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIncludingEx(this);
        }
	}

	#endregion

	#region IssetEx

	/// <summary>
	/// Represents <c>isset</c> construct.
	/// </summary>
	public sealed class IssetEx : Expression
	{
        public override Operations Operation { get { return Operations.Isset; } }

		private readonly IList<VariableUse>/*!*/ varList;
        /// <summary>List of variables to test</summary>
        public IList<VariableUse>/*!*/ VarList { get { return varList; } }

		public IssetEx(Text.Span span, IList<VariableUse>/*!*/ varList)
			: base(span)
		{
			Debug.Assert(varList != null && varList.Count > 0);
			this.varList = varList;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIssetEx(this);
        }
	}

	#endregion

	#region EmptyEx

	/// <summary>
	/// Represents <c>empty</c> construct.
	/// </summary>
	public sealed class EmptyEx : Expression
	{
        public override Operations Operation { get { return Operations.Empty; } }

        /// <summary>
        /// Expression to be checked for emptiness.
        /// </summary>
        public Expression/*!*/Expression { get { return this.expression; } set { this.expression = value; } }
        private Expression/*!*/expression;
        
        public EmptyEx(Text.Span p, Expression expression)
			: base(p)
		{
            if (expression == null)
                throw new ArgumentNullException("expression");

            this.expression = expression;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitEmptyEx(this);
        }
	}

	#endregion

	#region EvalEx, AssertEx

	/// <summary>
	/// Represents <c>eval</c> construct.
	/// </summary>
	public sealed class EvalEx : Expression
	{
        public override Operations Operation { get { return Operations.Eval; } }

		/// <summary>Expression containing source code to be evaluated.</summary>
        public Expression /*!*/ Code { get { return code; } set { code = value; } }

        /// <summary>
        /// Expression containing source code to be evaluated.
        /// </summary>
        private Expression/*!*/ code;
        
		#region Construction

		/// <summary>
		/// Creates a node representing an eval or assert constructs.
		/// </summary>
        /// <param name="span">Position.</param>
		/// <param name="code">Source code expression.</param>
		public EvalEx(Text.Span span, Expression/*!*/ code)
            : base(span)
		{
            this.code = code;
		}

		#endregion

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitEvalEx(this);
        }
	}

    /// <summary>
    /// Meta language element used for assert() function call analysis.
    /// </summary>
    public sealed class AssertEx : Expression
    {
        public override Operations Operation { get { return Operations.Eval; } }

        /// <summary>Expression containing source code to be evaluated.</summary>
        public Expression /*!*/ CodeEx { get; internal set; }

        public AssertEx(Text.Span span, CallSignature callsignature)
            : base(span)
        {
            Debug.Assert(callsignature.Parameters.Any());
            Debug.Assert(callsignature.GenericParams.IsEmpty());

            this.CodeEx = callsignature.Parameters[0].Expression;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitAssertEx(this);
        }
    }

	#endregion

	#region ExitEx

	/// <summary>
	/// Represents <c>exit</c> expression.
	/// </summary>
	public sealed class ExitEx : Expression
	{
        public override Operations Operation { get { return Operations.Exit; } }

		/// <summary>Die (exit) expression. Can be null.</summary>
        public Expression ResulExpr { get { return resultExpr; } set { resultExpr = value; } }
        private Expression resultExpr; //can be null
        
		public ExitEx(Text.Span span, Expression resultExpr)
            : base(span)
		{
			this.resultExpr = resultExpr;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitExitEx(this);
        }
	}

	#endregion
}
