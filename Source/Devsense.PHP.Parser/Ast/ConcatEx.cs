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
    /// Represents a concatenation expression ('<c>.</c>' PHP operator).
    /// </summary>
    public interface IConcatEx : IExpression
    {
        IReadOnlyList<IExpression> Expressions { get; }
    }

    /// <summary>
    /// Represents a concatenation expression ('<c>.</c>' PHP operator).
    /// </summary>
    public sealed class ConcatEx : Expression, IConcatEx
    {
        /// <summary>
        /// Operation used to concatenate the expressions.
        /// </summary>
        public override Operations Operation { get { return Operations.ConcatN; } }

        /// <summary>
        /// Expressions represented by the <see cref="ConcatEx"/>.
        /// </summary>
        public Expression[]/*!*/ Expressions { get { return _expressions; } }

        private Expression[]/*!*/ _expressions;

        IReadOnlyList<IExpression> IConcatEx.Expressions { get { return _expressions; } }

        /// <summary>
        /// Initialize the ConcatEx AST node and optimize the subtree if possible. Look for child expressions and chain possible concatenations. This prevents StackOverflowException in case of huge concatenation expressions.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="expressions">List of expressions to concatenate.</param>
        /// <remarks>This method tries to propagate child concatenations and chain them.</remarks>
        public ConcatEx(Text.Span span, IList<Expression>/*!*/ expressions)
            : base(span)
        {
            Debug.Assert(expressions != null);
            _expressions = expressions.AsArray();
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitConcatEx(this);
        }
    }
}

