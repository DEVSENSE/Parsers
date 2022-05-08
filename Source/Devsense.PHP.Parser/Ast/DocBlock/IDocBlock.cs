using Devsense.PHP.Syntax.Ast;

namespace Devsense.PHP.Ast.DocBlock
{
    /// <summary>
    /// Represents a PHP Documentary Comment.
    /// </summary>
    public interface IDocBlock : ILangElement
    {
        /// <summary>
        /// Gets the PHPDoc summary string.
        /// </summary>
        string Summary { get; }

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
