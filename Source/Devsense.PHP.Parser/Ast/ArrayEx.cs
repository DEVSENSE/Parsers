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
    /// Represents <c>array</c> or <c>list</c> constructs.
    /// </summary>
    public interface IArrayExpression : IExpression
    {
        /// <summary>
        /// Gets value indicating the array element is in form of short syntax (<c>[</c>, <c>]</c>).
        /// </summary>
        bool IsShortSyntax { get; }

        /// <summary>
        /// Enumeration of array items.
        /// </summary>
        ICollection<IArrayItem> Items { get; }
    }

    /// <summary>
    /// Represents an array or list item.
    /// </summary>
    public interface IArrayItem
    {

    }

    /// <summary>
    /// Represents <c>array</c> constructor.
    /// </summary>
    public sealed class ArrayEx : VarLikeConstructUse, IArrayExpression
    {
        [Flags]
        enum Flags
        {
            Array = 1,
            List = 2,
            ShortSyntax = 16,
        }

        readonly Item[]/*!*/_items;
        readonly Flags _flags;

        public override Operations Operation => ((_flags & Flags.Array) != 0) ? Operations.Array : Operations.List;

        internal override bool AllowsPassByReference { get { return false; } }

        /// <summary>
        /// Gets array items.
        /// </summary>
        public Item[]/*!*/ Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets value indicating the array element is in form of short syntax (<c>[</c>, <c>]</c>).
        /// </summary>
        public bool IsShortSyntax => (_flags & Flags.ShortSyntax) != 0;

        ICollection<IArrayItem> IArrayExpression.Items => _items;

        public static ArrayEx CreateArray(Text.Span span, IList<Item>/*!*/items, bool isShortSyntax)
            => new ArrayEx(span, items, Flags.Array | (isShortSyntax ? Flags.ShortSyntax : 0));

        public static ArrayEx CreateList(Text.Span span, IList<Item>/*!*/items, bool isShortSyntax)
            => new ArrayEx(span, items, Flags.List | (isShortSyntax ? Flags.ShortSyntax : 0));

        private ArrayEx(Text.Span span, IList<Item> items, Flags flags)
            : base(span)
        {
            Debug.Assert(flags != 0);

            _items = items.AsArray();
            _flags = flags;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            if ((_flags & Flags.Array) != 0)
            {
                visitor.VisitArrayEx(this);
            }
            else
            {
                visitor.VisitListEx(this);
            }
        }
    }

    #region Item

    /// <summary>
    /// Base class for item of an array defined by <c>array</c> constructor.
    /// </summary>
    public abstract class Item : AstNode, IArrayItem
    {
        /// <summary>
        /// The item key, can be <c>null</c>.
        /// </summary>
        public Expression Index { get { return index; } }
        readonly Expression index; // can be null

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
        public Expression ValueExpr { get { return valueExpr; } }
        readonly Expression valueExpr;

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
        /// <summary>Object to obtain reference of</summary>
        public VariableUse/*!*/RefToGet { get { return this.refToGet; } }
        readonly VariableUse/*!*/refToGet;

        public RefItem(Expression index, VariableUse refToGet)
            : base(index)
        {
            Debug.Assert(refToGet != null);
            this.refToGet = refToGet;
        }
    }

    #endregion

}