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

        /// <summary>
        /// <c>true</c> if the list is declared in the old notation 'list(...)', <c>false</c> if the new new notation '[...]' is used
        /// </summary>
        public bool IsOldNotation => _isOldNotation;
        private readonly bool _isOldNotation;

        public ListEx(Text.Span p, IList<Item>/*!*/lvalues, bool isOldNotation)
            : base(p)
        {
            Debug.Assert(lvalues != null);
            //Debug.Assert(lvalues.All(item => item is ValueItem && (
            //    ((ValueItem)item).ValueExpr == null ||
            //    ((ValueItem)item).ValueExpr is VarLikeConstructUse ||
            //    ((ValueItem)item).ValueExpr is ListEx)));

            _items = lvalues.AsArray();
            _isOldNotation = isOldNotation;
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
