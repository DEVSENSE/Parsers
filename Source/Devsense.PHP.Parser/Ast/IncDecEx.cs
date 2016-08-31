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

namespace Devsense.PHP.Syntax.Ast
{
	/// <summary>
	/// Post/pre increment/decrement expression.
	/// </summary>
	public sealed class IncDecEx : Expression
	{
        public override Operations Operation { get { return Operations.IncDec; } }

        [Flags]
        private enum Flags : byte
        {
            /// <summary>
            /// Indicates incrementation.
            /// </summary>
            incrementation = 1,

            /// <summary>
            /// Indicates post-incrementation or post-decrementation.
            /// </summary>
            post = 2,
        }

        private readonly Flags flags;

        /// <summary>Indicates incrementation.</summary>
        public bool Inc { get { return flags.HasFlag(Flags.incrementation); } }
		/// <summary>Indicates post-incrementation or post-decrementation</summary>
        public bool Post { get { return flags.HasFlag(Flags.post); } }

        private VariableUse/*!*/ variable;
        /// <summary>Variable being incremented/decremented</summary>
        public VariableUse /*!*/ Variable { get { return variable; } }

		public IncDecEx(Text.Span span, bool inc, bool post, VariableUse/*!*/ variable)
			: base(span)
		{
			this.variable = variable;

            if (inc) this.flags |= Flags.incrementation;
            if (post) this.flags |= Flags.post;
		}

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIncDecEx(this);
        }
	}
}
