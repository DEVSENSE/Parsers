using System;

namespace Devsense.PHP.Syntax.Ast
{
    public sealed class HaltCompiler : Statement
    {
        public HaltCompiler(Text.Span span) : base(span)
        {
        }

        public override void VisitMe(TreeVisitor visitor)
        {
            visitor.VisitHaltCompiler(this);
        }
    }
}
