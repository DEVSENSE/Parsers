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
    /// Represents <c>array</c> constructor.
    /// </summary>
    public sealed class ArrayEx : VarLikeConstructUse
    {
        public override Operations Operation { get { return Operations.Array; } }
        internal override bool AllowsPassByReference { get { return false; } }

        public Item[]/*!*/ Items
        {
            get { return items; }
            set { Debug.Assert(value != null); items = value; }
        }
        private Item[]/*!*/items;

        public ArrayEx(Text.Span span, IList<Item>/*!*/items)
            : base(span)
        {
            this.Items = items.AsArray();
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitArrayEx(this);
        }
    }

    #region Item

    /// <summary>
    /// Base class for item of an array defined by <c>array</c> constructor.
    /// </summary>
    public abstract class Item : AstNode, ISeparatedElements
    {
        public Expression Index { get { return index; } internal set { index = value; } }
        private Expression index; // can be null

        /// <summary>
        /// Position of the arrow operator, <c>-1</c> if not present.
        /// </summary>
        public int ArrowPosition
        {
            get
            {
                return (_arrowOffset < 0 && index == null) ? -1 :
                  Index.Span.Start + _arrowOffset;
            }
            set
            {
                _arrowOffset = value < 0 ?
                  (short)-1 : (short)(value - Index.Span.Start);
            }
        }
        public bool IsArrowPresent => _arrowOffset >= 0;
        private short _arrowOffset = -1;

        /// <summary>
        /// Position of the comma separator following the item, <c>-1</c> if not present.
        /// </summary>
        public virtual int SeparatorPosition { get; set; }

        protected Item(Expression index)
        {
            this.index = index;
        }

        internal bool HasKey { get { return (index != null); } }
        internal bool IsIndexLiteral { get { return index is Literal; } }
        internal bool IsIndexStringLiteral { get { return index is StringLiteral; } }
    }

    #endregion

    #region ValueItem

    /// <summary>
    /// Expression for the value of an array item defined by <c>array</c> constructor.
    /// </summary>
    public sealed class ValueItem : Item
    {
        /// <summary>Value of array item</summary>
        public Expression ValueExpr { get { return valueExpr; } internal set { valueExpr = value; } }
        private Expression valueExpr;

        /// <summary>
        /// Position of the comma separator following the item, <c>-1</c> if not present.
        /// </summary>
        public override int SeparatorPosition
        {
            get { return _separatorOffset < 0 ? -1 : valueExpr.Span.Start + _separatorOffset; }
            set { _separatorOffset = value < 0 ? (short)-1 : (short)(value - valueExpr.Span.Start); }
        }
        public bool IsSeparatorPresent => _separatorOffset >= 0;
        private short _separatorOffset = -1;

        public ValueItem(Expression index, Expression/*!*/ valueExpr)
            : base(index)
        {
            Debug.Assert(valueExpr != null);
            this.valueExpr = valueExpr;
        }
    }

    #endregion

    #region RefItem

    /// <summary>
    /// Reference to a variable containing the value of an array item defined by <c>array</c> constructor.
    /// </summary>
    public sealed class RefItem : Item
    {
        private readonly VariableUse/*!*/refToGet;
        /// <summary>Object to obtain reference of</summary>
        public VariableUse/*!*/RefToGet { get { return this.refToGet; } }

        /// <summary>
        /// Position of the comma separator following the item, <c>-1</c> if not present.
        /// </summary>
        public override int SeparatorPosition
        {
            get { return _separatorOffset < 0 ? -1 : RefToGet.Span.Start + _separatorOffset; }
            set { _separatorOffset = value < 0 ? (short)-1 : (short)(value - RefToGet.Span.Start); }
        }
        public bool IsSeparatorPresent => _separatorOffset >= 0;
        private short _separatorOffset = -1;

        /// <summary>
        /// Position of the reference operator.
        /// </summary>
        public int RefPosition
        {
            get { return refToGet.Span.Start + _refOffset; }
            set { _refOffset = (short)(value - refToGet.Span.Start); }
        }
        private short _refOffset = -1;

        public RefItem(Expression index, VariableUse refToGet)
            : base(index)
        {
            Debug.Assert(refToGet != null);
            this.refToGet = refToGet;
        }
    }

    #endregion

}