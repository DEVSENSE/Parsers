using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PHP.Core.Text;
using PHP.Core.AST;

namespace PhpParser.Parser
{
    public interface IReductionsSink
    {
        void InclusionReduced(Parser/*!*/ parser, IncludingEx/*!*/ decl);
        void FunctionDeclarationReduced(Parser/*!*/ parser, FunctionDecl/*!*/ decl);
        void TypeDeclarationReduced(Parser/*!*/ parser, TypeDecl/*!*/ decl);
        void GlobalConstantDeclarationReduced(Parser/*!*/ parser, GlobalConstantDecl/*!*/ decl);
        void NamespaceDeclReduced(Parser/*!*/ parser, NamespaceDecl/*!*/ decl);
        void LambdaFunctionReduced(Parser/*!*/ parser, LambdaFunctionExpr/*!*/ decl);
    }

    public partial class Parser
    {
        protected sealed override int EofToken
        {
            get { return (int)Tokens.END; }
        }

        protected sealed override int ErrorToken
        {
            get { return (int)Tokens.T_ERROR; }
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
