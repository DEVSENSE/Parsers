//#nullable enable

using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Represents a throw statement.
    /// </summary>
    public sealed class ThrowEx : Expression
    {
        int _span_start = -1;

        public override Span Span
        {
            get => _span_start < 0 ? Span.Invalid : Span.FromBounds(_span_start, Expression.Span.End);
            protected set => _span_start = value.IsValid ? value.Start : -1;
        }

        /// <summary>
        /// An expression being thrown.
        /// </summary>
        public Expression /*!*/ Expression { get; internal set; }

        /// <summary>
        /// Gets the expression operation.
        /// </summary>
        public override Operations Operation => Operations.Throw;

        public ThrowEx(Text.Span span, Expression/*!*/ expression)
            : base(span)
        {
            this.Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        /// <summary>
        /// Call the right Visit* method on the given Visitor object.
        /// </summary>
        /// <param name="visitor">Visitor to be called.</param>
        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitThrowEx(this);
        }
    }
}
