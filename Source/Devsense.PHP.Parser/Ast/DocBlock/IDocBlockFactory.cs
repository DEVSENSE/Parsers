using Devsense.PHP.Syntax;
using Devsense.PHP.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Ast.DocBlock
{
    /// <summary>
    /// A factory object for documentary comments.
    /// </summary>
    public interface IDocBlockFactory
    {
        /// <summary>
        /// Creates instance of documentary comment block.
        /// </summary>
        /// <param name="span">The position in source code.</param>
        /// <param name="source">The entire comment source text.</param>
        /// <returns>Instance of <see cref="IDocBlock"/>, or <c>null</c> so the documentary comment will be ignored for the further processing.</returns>
        IDocBlock CreateDocBlock(Span span, string source);
    }

    /// <summary>
    /// A default <see cref="IDocBlockFactory"/> implementation.
    /// </summary>
    public class DefaultDocBlockFactory : IDocBlockFactory
    {
        /// <summary>
        /// Singletong instance of this factory class.
        /// </summary>
        public static readonly IDocBlockFactory Instance = new DefaultDocBlockFactory();

        public IDocBlock CreateDocBlock(Span span, string source) => new PHPDocBlock(source, span);
    }
}
