using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PHP.Core.Text;

namespace PHP.Core.AST
{
    public sealed class HaltCompiler : LangElement
    {
        public HaltCompiler(Span span) : base(span)
        {
        }

        public override void VisitMe(TreeVisitor visitor)
        {
        }
    }
}
