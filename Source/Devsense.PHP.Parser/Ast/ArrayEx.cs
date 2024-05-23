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
    #region IArrayExpression, IArrayItem

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
        IReadOnlyList<IArrayItem> Items { get; }
    }

    /// <summary>
    /// Represents an array or list item.
    /// </summary>
    public interface IArrayItem
    {
        /// <summary>
        /// Gets value indicating that <see cref="Value"/> is passed by reference (<c>&amp;</c>).
        /// </summary>
        bool IsByRef { get; }

        bool IsSpreadItem { get; }

        /// <summary>
        /// Gets the item index, can be <c>null</c>.
        /// </summary>
        IExpression Index { get; }

        /// <summary>
        /// Gets the item value. Cannot be <c>null</c>.
        /// </summary>
        IExpression Value { get; }
    }

    /// <summary>
    /// Represents spread array item (<c>...</c> operator).
    /// </summary>
    public interface ISpreadArrayItem : IArrayItem
    {
    }

    #endregion

    #region ArrayEx

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

        public IReadOnlyList<IArrayItem> Items => _items;

        readonly IArrayItem[]/*!*/_items;
        
        readonly Flags _flags;

        public override Operations Operation => ((_flags & Flags.Array) != 0) ? Operations.Array : Operations.List;

        internal override bool AllowsPassByReference => false;

        public override Expression IsMemberOf => null;

        /// <summary>
        /// Gets value indicating the array element is in form of short syntax (<c>[</c>, <c>]</c>).
        /// </summary>
        public bool IsShortSyntax => (_flags & Flags.ShortSyntax) != 0;

        public static ArrayEx CreateArray(Text.Span span, IList<IArrayItem>/*!*/items, bool isShortSyntax)
            => new ArrayEx(span, items, Flags.Array | (isShortSyntax ? Flags.ShortSyntax : 0));

        public static ArrayEx CreateList(Text.Span span, IList<IArrayItem>/*!*/items, bool isShortSyntax)
            => new ArrayEx(span, items, Flags.List | (isShortSyntax ? Flags.ShortSyntax : 0));

        private ArrayEx(Text.Span span, IList<IArrayItem> items, Flags flags)
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

    #endregion

    #region Item

    /// <summary>
    /// Base class for item of an array defined by <c>array</c> constructor.
    /// </summary>
    abstract class Item : AstNode, IArrayItem
    {
        #region ValueItem

        /// <summary>
        /// Expression for the value of an array item defined by <c>array</c> constructor.
        /// </summary>
        sealed class ValueItem : Item
        {
            public override Expression Index { get; }

            /// <summary>Value of array item</summary>
            public Expression ValueExpr { get; }

            public override bool IsByRef => false;

            public override bool IsSpreadItem => false;

            public override IExpression Value => ValueExpr;

            public ValueItem(Expression index, Expression/*!*/ valueExpr)
            {
                this.Index = index;
                this.ValueExpr = valueExpr ?? throw new ArgumentNullException(nameof(valueExpr));
            }
        }

        #endregion

        #region SimpleValueItem

        sealed class SimpleValueItem : Item
        {
            public override Expression Index => null;

            public Expression ValueExpr => this.GetProperty<Expression>();

            public override IExpression Value => ValueExpr;

            public override bool IsByRef => false;

            public override bool IsSpreadItem => false;
            
            public SimpleValueItem(Expression value)
            {
                this.SetProperty<Expression>(value ?? throw new ArgumentNullException(nameof(value)));
            }
        }

        #endregion

        #region RefItem

        /// <summary>
        /// Reference to a variable containing the value of an array item defined by <c>array</c> constructor.
        /// </summary>
        sealed class RefItem : Item
        {
            /// <summary>Object to obtain reference of</summary>
            public VariableUse/*!*/RefToGet { get; }

            public override bool IsByRef => true;

            public override bool IsSpreadItem => false;

            public override Expression Index { get; }

            public override IExpression Value => RefToGet;

            public RefItem(Expression index, VariableUse refToGet)
            {
                this.Index = index;
                this.RefToGet = refToGet ?? throw new ArgumentNullException(nameof(refToGet));
            }
        }

        #endregion

        #region SpreadItem

        /// <summary>
        /// Expression to be spread into the array.
        /// <code>[...$expression]</code>
        /// </summary>
        sealed class SpreadItem : Item, ISpreadArrayItem
        {
            public override bool IsSpreadItem => true;

            public override Expression Index => null;

            /// <summary>
            /// Expression to be spread into the array.
            /// </summary>
            public Expression/*!*/Expression { get; }

            public SpreadItem(Expression expression)
            {
                this.Expression = expression;
            }

            public override bool IsByRef => false;

            public override IExpression Value => Expression;
        }

        #endregion

        public static Item/*!*/CreateValueItem(Expression index, Expression value) => index != null ? new ValueItem(index, value) : new SimpleValueItem(value);

        public static Item/*!*/CreateByRefItem(Expression index, VariableUse refToGet) => new RefItem(index, refToGet);

        public static Item/*!*/CreateSpreadItem(Expression expression) => new SpreadItem(expression);

        /// <summary>
        /// The item key, can be <c>null</c>.
        /// </summary>
        public abstract Expression Index { get; }

        public abstract bool IsByRef { get; }

        public abstract bool IsSpreadItem { get; }

        IExpression IArrayItem.Index => Index;

        public abstract IExpression Value { get; }
    }

    #endregion
}