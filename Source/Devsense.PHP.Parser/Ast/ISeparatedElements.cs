using System;
using System.Collections.Generic;
using System.Text;

namespace Devsense.PHP.Syntax.Ast
{
    public interface ISeparatedElements
    {
        /// <summary>
        /// Position of the separator following the item, <c>-1</c> if not present.
        /// </summary>
        int SeparatorPosition { get; set; }
    }
    public interface IInitializedElements
    {
        /// <summary>
        /// Position of the separator following the item, <c>-1</c> if not present.
        /// </summary>
        int AssignmentPosition { get; set; }
    }
    public interface IAliasReturn
    {
        /// <summary>
        /// Position of the alias symbol, <c>-1</c> if not present.
        /// </summary>
        int ReferencePosition { get; set; }
    }
}
