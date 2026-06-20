using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Syntax
{
    /// <summary>
    /// Fake string element representing not-yet processed <see cref="Tokens.T_ENCAPSED_AND_WHITESPACE"/> token value.
    /// </summary>
    internal sealed class EncapsedStringElement : Expression
    {
        public ReadOnlyMemory<char> TokenSource { get; }

        public EncapsedStringElement(Span span, ReadOnlyMemory<char> source) : base(span)
        {
            this.TokenSource = source;
        }

        public override Operations Operation => Operations.StringLiteral;

        public override Span Span { get; protected set; }

        public override void VisitMe(TreeVisitor visitor) => throw new NotSupportedException();
    }
}
