using Devsense.PHP.Syntax.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.TestImplementation
{
    internal static class SyntaxExtensions
    {
        /// <summary>
        /// Enumerates all the type declarations (excluding anonymous classes).
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<NamedTypeDecl> TraverseNamedTypeDeclarations(this GlobalCode/*!*/ast)
        {
            if (ast == null) throw new ArgumentNullException(nameof(ast));

            var todo = new Stack<IBlockStatement>();
            todo.Push(ast);

            while (todo.Count != 0)
            {
                var stmts = todo.Pop().Statements;
                foreach (var stmt in stmts)
                {
                    if (stmt is NamedTypeDecl tdecl)
                    {
                        yield return tdecl;
                    }
                    else if (stmt is NamespaceDecl nsdecl)
                    {
                        todo.Push(nsdecl.Body);
                    }
                }
            }

            yield break;
        }
    }
}
