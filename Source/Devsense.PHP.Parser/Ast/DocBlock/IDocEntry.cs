using Devsense.PHP.Text;

namespace Devsense.PHP.Ast.DocBlock
{
    /// <summary>
    /// A documentary comment entry within <see cref="IDocBlock"/>.
    /// </summary>
    public interface IDocEntry
    {
        ///// <summary>
        ///// Gets the entry name (i.e. the PHPDoc keyword without the <c>@</c> prefix).
        ///// </summary>
        //string Name { get; }

        /// <summary>
        /// The entry position in source code.
        /// </summary>
        Span Span { get; }

        /// <summary>
        /// Gets the string representation of the entry.
        /// </summary>
        string ToString();

        /// <summary>
        /// The next sibling entry.
        /// Can be <c>null</c> if there are no more entries.
        /// </summary>
        IDocEntry Next { get; }
    }

    internal class CommonDocEntry : IDocEntry
    {
        public Span Span { get; set; }

        public IDocEntry Next { get; set; }

        public string Content { get; set; }

        public override string ToString() => Content;
    }
}
