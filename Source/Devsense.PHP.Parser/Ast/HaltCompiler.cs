using System;

namespace Devsense.PHP.Syntax.Ast
{
    public sealed class HaltCompiler : LangElement
    {
        public HaltCompiler(Text.Span span) : base(span)
        {
        }

        public override void VisitMe(TreeVisitor visitor)
        {
        }
    }
}
