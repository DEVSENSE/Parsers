using PHP.Core.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
