using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public List<Item>/*!*/LValues { get; private set; }
        /// <summary>Array being assigned</summary>
        public Expression RValue { get; internal set; }

        public ListEx(Text.Span p, List<Item>/*!*/ lvalues)
            : base(p)
        {
            Debug.Assert(lvalues != null);
            //Debug.Assert(lvalues.TrueForAll(delegate(Expression lvalue)
            //{
            //    return lvalue. == null || lvalue is VarLikeConstructUse || lvalue is ListEx;
            //}));

            this.LValues = lvalues;
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
