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

using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

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
        /// Array items.
        /// </summary>
        ReadOnlySpan<ArrayItem> Items { get; }
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

        public ReadOnlySpan<ArrayItem> Items => _items.AsSpan();

        readonly ArrayItem[]/*!*/_items;

        readonly Flags _flags;

        public override Span Span { get; protected set; }

        public override Operations Operation => ((_flags & Flags.Array) != 0) ? Operations.Array : Operations.List;

        internal override bool AllowsPassByReference => false;

        public override Expression IsMemberOf => null;

        /// <summary>
        /// Gets value indicating the array element is in form of short syntax (<c>[</c>, <c>]</c>).
        /// </summary>
        public bool IsShortSyntax => (_flags & Flags.ShortSyntax) != 0;

        public static ArrayEx CreateArray(Text.Span span, ArrayItem[]/*!*/items, bool isShortSyntax)
            => new ArrayEx(span, items, Flags.Array | (isShortSyntax ? Flags.ShortSyntax : 0));

        public static ArrayEx CreateList(Text.Span span, ArrayItem[]/*!*/items, bool isShortSyntax)
            => new ArrayEx(span, items, Flags.List | (isShortSyntax ? Flags.ShortSyntax : 0));

        private ArrayEx(Text.Span span, ArrayItem[] items, Flags flags)
            : base(span)
        {
            Debug.Assert(flags != 0);

            _items = items ?? EmptyArray<ArrayItem>.Instance;
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

    #region ArrayItem

    /// <summary>
    /// Indexed array item.
    /// </summary>
    public readonly struct ArrayItem
    {
        //public static ArrayItem[]/*!*/EmptyArray => Array.Empty<ArrayItem>();
        //internal static readonly object/*!*/BoxedDefault = (object)default(ArrayItem);

        [Flags]
        enum ArrayItemFlags : byte
        {
            None = 0,
            ByRef = 1,
            Spread = 1 << 1,
        }

        readonly ArrayItemFlags _flags;
        readonly IExpression _value;
        readonly IExpression _index;

        /// <summary>
        /// Optional. Array item index expression.
        /// </summary>
        public IExpression Index => _index;

        /// <summary>
        /// Array item value expression.
        /// </summary>
        public IExpression Value => _value;

        public bool IsByRef => (_flags & ArrayItemFlags.ByRef) != 0;

        public bool IsSpreadItem => (_flags & ArrayItemFlags.Spread) != 0;

        /// <summary>
        /// The value is not initialized.
        /// </summary>
        public bool IsDefault => ReferenceEquals(_value, null) && ReferenceEquals(_index, null);

        internal void Compress(out object o1, out object o2, out byte flags)
        {
            o1 = _index;
            o2 = _value;
            flags = (byte)_flags;
        }

        internal static ArrayItem CreateFromCompressed(object o1, object o2, byte flags)
        {
            return (o1 == null && o2 == null && flags == 0)
                ? default(ArrayItem) // null item
                : new ArrayItem((IExpression)o1, (IExpression)o2, (ArrayItemFlags)flags)
                ;
        }

        ArrayItem(IExpression index, IExpression value, ArrayItemFlags flags)
        {
            _index = index;
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _flags = flags;
        }

        public static ArrayItem/*!*/CreateValueItem(Expression index, Expression value) => new ArrayItem(index, value, ArrayItemFlags.None);

        public static ArrayItem/*!*/CreateByRefItem(Expression index, VariableUse refToGet) => new ArrayItem(index, refToGet, ArrayItemFlags.ByRef);

        public static ArrayItem/*!*/CreateSpreadItem(Expression expression) => new ArrayItem(null, expression, ArrayItemFlags.Spread);
    }

    #endregion
}