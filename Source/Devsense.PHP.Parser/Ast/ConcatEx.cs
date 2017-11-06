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
    /// Represents a concatenation expression (dot PHP operator).
    /// </summary>
    public sealed class ConcatEx : Expression
    {
        private string _label;

        /// <summary>
        /// Token used to initiate the concat expression.
        /// </summary>
        public Tokens OpenToken => _label.Length == 1 ? (Tokens)_label[0] : Tokens.T_START_HEREDOC;

        /// <summary>
        /// Token used to terminate the concat expression.
        /// </summary>
        public Tokens CloseToken => _label.Length == 1 ? (Tokens)_label[0] : Tokens.T_END_HEREDOC;

        /// <summary>
        /// Text initiate the concat (label for heredoc).
        /// </summary>
        public string OpenLabel => _label.Length == 1 ? _label : $"<<<{_label}";

        /// <summary>
        /// Text terminating the concat.
        /// </summary>
        public string CloseLabel => _label;


        /// <summary>
        /// Operation used to concatenate the expressions.
        /// </summary>
        public override Operations Operation { get { return Operations.ConcatN; } }

        /// <summary>
        /// Expressions represented by the <see cref="ConcatEx"/>.
        /// </summary>
        public Expression[]/*!*/ Expressions { get { return this._expressions; } internal set { this._expressions = value; } }
        private Expression[]/*!*/ _expressions;

        /// <summary>
        /// Initialize the ConcatEx AST node and optimize the subtree if possible. Look for child expressions and chain possible concatenations. This prevents StackOverflowException in case of huge concatenation expressions.
        /// </summary>
        /// <param name="span"></param>
        /// <param name="expressions">List of expressions to concatenate.</param>
        /// <param name="label">Label used to initiate and terminate the concatenation.</param>
        /// <remarks>This method tries to propagate child concatenations and chain them.</remarks>
        public ConcatEx(Text.Span span, IList<Expression>/*!*/ expressions, string/*!*/label)
            : base(span)
        {
            Debug.Assert(expressions != null);
            Debug.Assert(!string.IsNullOrEmpty(label));
            this._expressions = expressions.AsArray();
            this._label = label;
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

