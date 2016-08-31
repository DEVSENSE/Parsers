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
	/// Represents a content of backtick operator (shell command execution).
	/// </summary>
	public sealed class ShellEx : Expression
	{
        public override Operations Operation { get { return Operations.ShellCommand; } }

		/// <summary>Command to excute</summary>
        public Expression/*!*/ Command { get { return command; } internal set { command = value; } }
        private Expression/*!*/ command;
        
		public ShellEx(Text.Span span, Expression/*!*/ command)
            : base(span)
		{
            Debug.Assert(command is StringLiteral || command is ConcatEx || command is BinaryStringLiteral);
			this.command = command;
		}

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitShellEx(this);
        }
	}
}
