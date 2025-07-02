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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

namespace Devsense.PHP.Syntax.Ast
{
    public interface IIfStmt : IStatement
    {
        IIfBranch Branches { get; }
    }

    public interface IIfBranch : IAstNode
    {
        /// <summary>
        /// Condition or a <B>null</B> reference for the case of "else" branch.
        /// </summary>
        IExpression Condition { get; }

        /// <summary>
        /// The branch statement.
        /// </summary>
        IStatement Statement { get; }

        /// <summary>
        /// Next branch.
        /// </summary>
        IIfBranch Else { get; }
    }

    /// <summary>
    /// Represents an if-statement.
    /// </summary>
    public sealed class IfStmt : Statement, IIfStmt
    {
        /// <summary>
        /// Linked list of conditions including the if-conditions and the final else.
        /// </summary>
        public ConditionalStmt/*!!*/ Conditions { get; internal set; }

        IIfBranch IIfStmt.Branches => this.Conditions;

        public IfStmt(Text.Span span, ConditionalStmt/*!!*/ conditions)
            : base(span)
        {
            Debug.Assert(conditions != null);
            this.Conditions = conditions;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitIfStmt(this);
        }
    }

    public sealed class ConditionalStmt : AstNode, IIfBranch
    {
        /// <summary>
        /// Beginning of <see cref="ConditionalStmt"/>.
        /// </summary>
        public readonly Text.Span Span;

        /// <summary>
        /// Condition or a <B>null</B> reference for the case of "else" branch.
        /// </summary>
        public Expression Condition { get; }

        /// <summary>
        /// Next branch.
        /// </summary>
        public ConditionalStmt Else { get; }

        /// <summary>
        /// Position of the header parentheses encapsulating <see cref="Condition"/>.
        /// If it is "else" branch with no <see cref="Condition"/>, it is set to <see cref="Text.Span.Invalid"/>.
        /// </summary>
        public Text.Span ParenthesesSpan { get; }

        /// <summary>
        /// The branch statement.
        /// </summary>
        public Statement/*!*/ Statement { get; internal set; }

        #region IIfBranch

        IExpression IIfBranch.Condition => this.Condition;

        IStatement IIfBranch.Statement => this.Statement;

        IIfBranch IIfBranch.Else => this.Else;

        #endregion

        public ConditionalStmt(Text.Span span, Expression condition, Text.Span parenthesesSpan, Statement/*!*/ statement, ConditionalStmt @else)
        {
            this.Span = span;
            this.Condition = condition;
            this.ParenthesesSpan = parenthesesSpan;
            this.Statement = statement;
            this.Else = @else;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        internal void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitConditionalStmt(this);
        }

        #region Enumerator, GetEnumerator

        public struct Enumerator : IEnumerator<ConditionalStmt>
        {
            private ConditionalStmt current;

            private ConditionalStmt next;

            public Enumerator(ConditionalStmt first)
            {
                this.current = null;
                this.next = first;
            }

            public ConditionalStmt Current => current;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                current = next;
                next = next?.Else;
                return current != null;
            }

            public void Reset()
            {
                current = null;
                next = null;
            }

            public void Dispose() { }
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        #endregion
    }
}
