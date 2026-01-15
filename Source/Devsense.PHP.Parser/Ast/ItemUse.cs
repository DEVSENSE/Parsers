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
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax.Ast
{
    #region ItemUse

    /// <summary>
	/// Access to an item of a structured variable by [] PHP operator.
	/// </summary>
	public abstract class ItemUse : CompoundVarUse
    {
        public override Span Span { get; protected set; }

        class SimpleItemUse : ItemUse
        {
            public SimpleItemUse(Span p, Expression array, Expression index) : base(p, array, index)
            {
            }
        }

        class ComplexItemUse : ItemUse
        {
            public override bool IsFunctionArrayDereferencing { get; }

            public override bool IsBraces { get; }

            public ComplexItemUse(Span p, Expression array, Expression index, bool functionArrayDereferencing, bool isBraces) : base(p, array, index)
            {
                this.IsFunctionArrayDereferencing = functionArrayDereferencing;
                this.IsBraces = isBraces;
            }
        }

        public override Operations Operation { get { return Operations.ItemUse; } }

        /// <remarks>Always <c>null</c>.</remarks>
        public override Expression IsMemberOf => null;

        /// <summary>
        /// Whether this represents function array dereferencing.
        /// </summary>
        public virtual bool IsFunctionArrayDereferencing => false;

        /// <summary>
        /// <c>True</c> if the array is accessed using '{}', false if using '[]'.
        /// </summary>
        public virtual bool IsBraces => false;

        /// <summary>
        /// Variable used as an array identifier.
        /// </summary>
        public Expression Array { get; }

        /// <summary>
        /// Expression used as an array index. 
        /// A <B>null</B> reference means key-less array operator (write context only).
        /// </summary>
        public Expression Index { get; }

        /// <summary>
        /// Create instance of <see cref="ItemUse"/>.
        /// </summary>
        public static ItemUse/*!*/Create(Span p, Expression/*!*/ array, Expression index, bool functionArrayDereferencing = false, bool isBraces = false)
        {
            if (isBraces == false && functionArrayDereferencing == false)
            {
                // common case:
                return new SimpleItemUse(p, array, index);
            }

            return new ComplexItemUse(
                p,
                array,
                index,
                functionArrayDereferencing: functionArrayDereferencing,
                isBraces: isBraces
            );
        }

        protected ItemUse(Span p, Expression/*!*/ array, Expression index)
            : base(p)
        {
            Debug.Assert(array != null);

            this.Array = array;
            this.Index = index;
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
    public sealed class StringLiteralDereferenceEx : ExpressionEntireSpan
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
