using Devsense.PHP.Syntax.Ast;
using System;

namespace Devsense.PHP.Ast.DocBlock
{
    /// <summary>
    /// Represents a PHP Documentary Comment.
    /// </summary>
    public interface IDocBlock : ILangElement
    {
        [Obsolete("Use HasSummary instead to avoid unnecessary allocations if actual summary is not needed.")]
        string Summary { get; }

        /// <summary>
        /// Gets value indicating the doc block has a non-empty summary text.
        /// </summary>
        bool HasSummary();

        /// <summary>
        /// Gets the PHPDoc summary string.
        /// </summary>
        bool HasSummary(out string summary);

        /// <summary>
        /// Gets entries within the documentary comment, as a linked list.
        /// </summary>
        IDocEntry Entries { get; }

        /// <summary>
        /// Gets the enumeration of <see cref="IDocEntry"/> list.
        /// </summary>
        DocBlockEntriesEnumerator<IDocEntry> GetEnumerator();
    }
}
