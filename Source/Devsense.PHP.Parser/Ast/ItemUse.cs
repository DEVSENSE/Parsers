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
using System.IO;
using System.Diagnostics;

namespace Devsense.PHP.Syntax.Ast
{
    #region ItemUse

    /// <summary>
	/// Access to an item of a structured variable by [] PHP operator.
	/// </summary>
	public sealed class ItemUse : CompoundVarUse
	{
        public override Operations Operation { get { return Operations.ItemUse; } }

        /// <summary>
        /// Whether this represents function array dereferencing.
        /// </summary>
        public bool IsFunctionArrayDereferencing { get { return this.functionArrayDereferencing; } }
        private readonly bool functionArrayDereferencing = false;

		/// <summary>
		/// Variable used as an array identifier.
		/// </summary>
        public VarLikeConstructUse Array { get { return array; } set { array = value; } }
        private VarLikeConstructUse/*!*/ array;

		/// <summary>
		/// Expression used as an array index. 
		/// A <B>null</B> reference means key-less array operator (write context only).
		/// </summary>
        public Expression Index { get { return index; } internal set { index = value; } }
		private Expression index;

        public ItemUse(Text.Span p, VarLikeConstructUse/*!*/ array, Expression index, bool functionArrayDereferencing = false)
			: base(p)
		{
			Debug.Assert(array != null);

			this.array = array;
			this.index = index;
            this.functionArrayDereferencing = functionArrayDereferencing;
		}

		/// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitItemUse(this);
        }
	}

    #endregion

    #region StringLiteralDereferenceEx

    /// <summary>
    /// String literal dereferencing.
    /// </summary>
    public sealed class StringLiteralDereferenceEx : Expression
    {
        public override Operations Operation
        {
            get { return Operations.ItemUse; }
        }

        /// <summary>
        /// Expression representing the string value.
        /// </summary>
        public Expression/*!*/StringExpr { get; internal set; }

        /// <summary>
        /// Expression representing index in the string.
        /// </summary>
        public Expression/*!*/KeyExpr { get; internal set; }

        public StringLiteralDereferenceEx(Text.Span span, Expression expr, Expression key)
            : base(span)
        {
            this.StringExpr = expr;
            this.KeyExpr = key;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitStringLiteralDereferenceEx(this);
        }
    }

    #endregion
}
