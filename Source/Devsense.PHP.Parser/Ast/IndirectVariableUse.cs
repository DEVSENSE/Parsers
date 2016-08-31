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
	/// Indirect variable use - a variable or a field access by run-time evaluated name.
	/// </summary>
	public sealed class IndirectVarUse : SimpleVarUse
	{
        public override Operations Operation { get { return Operations.IndirectVarUse; } }

		public Expression VarNameEx { get { return varNameEx; } }
		internal Expression varNameEx;

		public IndirectVarUse(Text.Span span, int levelOfIndirection, Expression varNameEx)
            : base(span)
		{
			Debug.Assert(levelOfIndirection > 0 && varNameEx != null);

			if (levelOfIndirection == 1)
			{
				this.varNameEx = varNameEx;
			}
			else
			{
                Text.Span varspan = new Text.Span(span.Start + 1, span.Length - 1);
                this.varNameEx = new IndirectVarUse(varspan, --levelOfIndirection, varNameEx);
			}
		}

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIndirectVarUse(this);
        }

	}
}
