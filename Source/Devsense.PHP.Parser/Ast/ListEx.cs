using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Devsense.PHP.Syntax.Ast
{
	/// <summary>
	/// Represents a <c>list</c> construct.
	/// </summary>
	public sealed class ListEx : VarLikeConstructUse
	{
        public override Operations Operation { get { return Operations.List; } }

        /// <summary>
        /// Elements of this list are VarLikeConstructUse, ListEx and null.
        /// Null represents empty expression - for example next piece of code is ok: 
        /// list(, $value) = each ($arr)
        /// </summary>
        public Item[]/*!*/ Items => _items;
        private readonly Item[]/*!*/_items;

        public ListEx(Text.Span p, IList<Item>/*!*/lvalues)
            : base(p)
        {
            Debug.Assert(lvalues != null);
            Debug.Assert(lvalues.All(item => item is ValueItem && (
                ((ValueItem)item).ValueExpr == null ||
                ((ValueItem)item).ValueExpr is VarLikeConstructUse ||
                ((ValueItem)item).ValueExpr is ListEx)));

            _items = lvalues.AsArray();
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitListEx(this);
        }
	}
}
