using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PHP.Core.Text;

namespace PhpParser.Parser
{
    public partial class Parser
    {
        protected sealed override int EofToken
        {
            get { return (int)Tokens.EOF; }
        }

        protected sealed override int ErrorToken
        {
            get { return (int)Tokens.ERROR; }
        }

        protected override Span CombinePositions(Span first, Span last)
        {
            if (last.IsValid)
            {
                if (first.IsValid)
                    return Span.Combine(first, last);
                else
                    return last;
            }
            else
                return first;
        }
    }
}
