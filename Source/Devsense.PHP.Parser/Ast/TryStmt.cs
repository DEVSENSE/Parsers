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
    /// Represents a try-catch statement.
    /// </summary>
    public sealed class TryStmt : Statement
    {
        /// <summary>
        /// A block containing the list of statements contained in the try-block.
        /// </summary>
        public BlockStmt/*!*/ Body { get { return body; } }
        private readonly BlockStmt/*!*/ body;

        /// <summary>
        /// A list of catch statements catching exceptions thrown inside the try block. Can be a <c>null</c> reference.
        /// </summary>
        private readonly CatchItem[]/*!*/catches;
        /// <summary>A list of catch statements catching exceptions thrown inside the try block.</summary>
        public CatchItem[]/*!*/Catches { get { return catches; } }
        internal bool HasCatches { get { return catches.Length != 0; } }

        /// <summary>
        /// A list of statements contained in the finally-block. Can be a <c>null</c> reference.
        /// </summary>
        private readonly FinallyItem finallyItem;
        /// <summary>A list of statements contained in the finally-block. Can be a <c>null</c> reference.</summary>
        public FinallyItem FinallyItem { get { return finallyItem; } }
        internal bool HasFinallyStatements { get { return finallyItem != null && finallyItem.Body.Statements.Length != 0; } }

        public TryStmt(Text.Span p, BlockStmt/*!*/ body, IList<CatchItem> catches, FinallyItem finallyItem)
            : base(p)
        {
            Debug.Assert(body != null);

            this.body = body;
            this.catches = catches.AsArray();
            this.finallyItem = finallyItem;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitTryStmt(this);
        }
    }

    /// <summary>
    /// Represents a catch-block.
    /// </summary>
    public sealed class CatchItem : LangElement
    {
        /// <summary>
        /// A block containing the list of statements contained in the catch-block.
        /// </summary>
        public BlockStmt/*!*/ Body { get { return body; } }
        private readonly BlockStmt/*!*/ body;

        /// <summary>
        /// A variable where an exception is assigned in.
        /// Cannot be <c>null</c>.
        /// </summary>
        private readonly DirectVarUse/*!*/ variable;
        /// <summary>A variable where an exception is assigned in.</summary>
        public DirectVarUse/*!*/ Variable { get { return variable; } }

        /// <summary>
        /// Optional. Catch variable type reference.
        /// Can be a multiple type reference.
        /// </summary>
        public TypeRef TargetType { get { return this.tref; } }
        private readonly TypeRef tref;

        public CatchItem(Text.Span p, TypeRef tref, DirectVarUse/*!*/ variable,
            BlockStmt body)
            : base(p)
        {
            Debug.Assert(variable != null && body != null);

            this.tref = tref;
            this.variable = variable;
            this.body = body;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitCatchItem(this);
        }
    }

    /// <summary>
    /// Represents a finally-block.
    /// </summary>
    public sealed class FinallyItem : LangElement
    {
        /// <summary>
        /// Statements in the finally block
        /// </summary>
        public BlockStmt/*!*/ Body { get { return body; } }
        private readonly BlockStmt/*!*/ body;

        public FinallyItem(Text.Span span, BlockStmt body)
            : base(span)
        {
            this.body = body;
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitFinallyItem(this);
        }
    }

    /// <summary>
    /// Represents a throw statement.
    /// </summary>
    public sealed class ThrowStmt : Statement
    {
        /// <summary>
        /// An expression being thrown.
        /// </summary>
        public Expression /*!*/ Expression { get { return expression; } internal set { expression = value; } }
        private Expression/*!*/ expression;

        public ThrowStmt(Text.Span span, Expression/*!*/ expression)
            : base(span)
        {
            Debug.Assert(expression != null);
            this.expression = expression;
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitThrowStmt(this);
        }
    }
}
