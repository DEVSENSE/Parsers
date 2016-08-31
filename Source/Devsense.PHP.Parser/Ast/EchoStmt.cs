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
	/// <summary>
	/// Represents an <c>echo</c> statement.
	/// </summary>
	public sealed class EchoStmt : Statement
	{
		/// <summary>Array of parameters - Expressions.</summary>
        public Expression[] /*!*/ Parameters { get { return parameters; } }
        private Expression[]/*!*/ parameters;
        
        /// <summary>
        /// Gets value indicating whether this <see cref="EchoStmt"/> represents HTML code.
        /// </summary>
        public bool IsHtmlCode { get { return isHtmlCode; } }
        private readonly bool isHtmlCode;

		public EchoStmt(Text.Span span, IList<Expression>/*!*/ parameters)
            : base(span)
		{
			Debug.Assert(parameters != null);
			this.parameters = parameters.AsArray();
            this.isHtmlCode = false;
		}

        /// <summary>
        /// Initializes new echo statement as a representation of HTML code.
        /// </summary>
        public EchoStmt(Text.Span span, string htmlCode)
            : base(span)
        {
            this.parameters = new Expression[] { new StringLiteral(span, htmlCode) };
            this.isHtmlCode = true;
        }

		internal override bool SkipInPureGlobalCode()
		{
			StringLiteral literal;
			if (parameters.Length == 1 && (literal = parameters[0] as StringLiteral) != null)
			{
				return StringUtils.IsWhitespace((string)literal.Value);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitEchoStmt(this);
        }
	}
}