using System;

namespace Devsense.PHP.Syntax.Ast
{
    /// <summary>
    /// Represents PHP <c> __halt_compiler</c> construct.
    /// </summary>
    public sealed class HaltCompiler : Statement
    {
        public HaltCompiler(Text.Span span)
            : base(span)
        {
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitHaltCompiler(this);
        }
    }
}
